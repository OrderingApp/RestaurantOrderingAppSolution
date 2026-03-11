using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database;
using Infrastructure.Database.Seeder;
using Infrastructure.Database.Seeder.Seeders;

Console.WriteLine("Seeder runner (dev-only)");

if (args.Length > 0 && args[0] == "--help")
{
    Console.WriteLine("Usage: dotnet run --project tools/seeder-runner/SeederRunner.csproj -- [sqliteFilePath]");
    Environment.Exit(0);
}

var withRelations = args.Any(a => a.Equals("--with-relations", StringComparison.OrdinalIgnoreCase));
var positional = args.Where(a => !a.StartsWith("--")).ToArray();
var dbFile = positional.Length > 0 ? positional[0] : "seeder-test.db";
var contentRoot = positional.Length > 1 ? positional[1] : null;
var services = new ServiceCollection();

services.AddDatabaseSeeders();

services.AddDbContext<RestaurantOrderingContext>(opts =>
{
    opts.UseSqlite($"Data Source={dbFile}");
});

// Provide a minimal IWebHostEnvironment for seeders that expect it (dev-only)
var env = new SimpleWebHostEnvironment
{
    ContentRootPath = contentRoot ?? Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Infrastructure.Database")
};

services.AddSingleton<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(env);

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
var db = scope.ServiceProvider.GetRequiredService<RestaurantOrderingContext>();
// Dev-runner: ensure database exists (creates schema for simple tests). For production use migrations.
db.Database.EnsureCreated();

Console.WriteLine($"Ready to run DatabaseSeeder against SQLite file: {dbFile}");
Console.WriteLine("To execute seeding, run this command with no additional args. Press Enter to proceed or Ctrl+C to abort.");
Console.ReadLine();

try
{
    // Run only a subset of seeders for the first smoke test
    var runResults = new List<(string Name, bool Success, string? Error, int Count)>();

    var menuCategorySeeder = scope.ServiceProvider.GetRequiredService<MenuCategorySeeder>();
    var subCategorySeeder = scope.ServiceProvider.GetRequiredService<SubCategorySeeder>();
    var ingredientSeeder = scope.ServiceProvider.GetRequiredService<IngredientSeeder>();
    var tagSeeder = scope.ServiceProvider.GetRequiredService<TagSeeder>();
    var areaSeeder = scope.ServiceProvider.GetRequiredService<AreaSeeder>();
    var tableSeeder = scope.ServiceProvider.GetRequiredService<TableSeeder>();
    var menuItemSeeder = scope.ServiceProvider.GetRequiredService<MenuItemSeeder>();

    async Task RunSeederAsync(string name, Func<Task> action, Func<Task<int>> countFunc)
    {
        try
        {
            await action();
            var cnt = await countFunc();
            runResults.Add((name, true, null, cnt));
            Console.WriteLine($"{name}: succeeded, records in DB = {cnt}");
        }
        catch (Exception ex)
        {
            runResults.Add((name, false, ex.Message, 0));
            Console.WriteLine($"{name}: failed - {ex.Message}");
        }
    }

    await RunSeederAsync("MenuCategorySeeder", () => menuCategorySeeder.SeedAsync(), () => db.MenuCategories.CountAsync());
    await RunSeederAsync("SubCategorySeeder", () => subCategorySeeder.SeedAsync(), () => db.SubCategories.CountAsync());
    await RunSeederAsync("IngredientSeeder", () => ingredientSeeder.SeedAsync(), () => db.Ingredients.CountAsync());
    await RunSeederAsync("TagSeeder", () => tagSeeder.SeedAsync(), () => db.Tags.CountAsync());
    await RunSeederAsync("AreaSeeder", () => areaSeeder.SeedAsync(), () => db.Areas.CountAsync());
    await RunSeederAsync("TableSeeder", () => tableSeeder.SeedAsync(), () => db.Tables.CountAsync());
    await RunSeederAsync("MenuItemSeeder", () => menuItemSeeder.SeedAsync(), () => db.MenuItems.CountAsync());

    Console.WriteLine("\nSeed run summary:");
    foreach (var r in runResults)
    {
        if (r.Success)
            Console.WriteLine($"  {r.Name}: OK, records = {r.Count}");
        else
            Console.WriteLine($"  {r.Name}: FAILED, error = {r.Error}");
    }

    if (withRelations)
    {
        Console.WriteLine("\n--with-relations flag detected: running relational seeders (IngredientTagRel, MenuItemIngredientRel)");
        var relResults = new List<(string Name, bool Success, string? Error, int Added, int Skipped)>();

        var ingredientTagRelSeeder = scope.ServiceProvider.GetRequiredService<IngredientTagRelSeeder>();
        var menuItemIngredientRelSeeder = scope.ServiceProvider.GetRequiredService<MenuItemIngredientRelSeeder>();

        try
        {
            var rep1 = await ingredientTagRelSeeder.SeedRelationsAsync();
            var total1 = await db.IngredientTagRels.CountAsync();
            relResults.Add(("IngredientTagRelSeeder", true, null, rep1.AddedCount, rep1.SkippedCount));
            Console.WriteLine($"IngredientTagRelSeeder: added={rep1.AddedCount}, skipped={rep1.SkippedCount}, total in DB={total1}");
        }
        catch (Exception ex)
        {
            relResults.Add(("IngredientTagRelSeeder", false, ex.Message, 0, 0));
            Console.WriteLine($"IngredientTagRelSeeder failed: {ex.Message}");
        }

        try
        {
            var rep2 = await menuItemIngredientRelSeeder.SeedRelationsAsync();
            var total2 = await db.MenuItemIngredientRels.CountAsync();
            relResults.Add(("MenuItemIngredientRelSeeder", true, null, rep2.AddedCount, rep2.SkippedCount));
            Console.WriteLine($"MenuItemIngredientRelSeeder: added={rep2.AddedCount}, skipped={rep2.SkippedCount}, total in DB={total2}");
        }
        catch (Exception ex)
        {
            relResults.Add(("MenuItemIngredientRelSeeder", false, ex.Message, 0, 0));
            Console.WriteLine($"MenuItemIngredientRelSeeder failed: {ex.Message}");
        }

        Console.WriteLine("\nRelational seed summary:");
        foreach (var r in relResults)
        {
            if (r.Success)
                Console.WriteLine($"  {r.Name}: OK, added={r.Added}, skipped={r.Skipped}");
            else
                Console.WriteLine($"  {r.Name}: FAILED, error={r.Error}");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Seeding failed: {ex.Message}");
}
