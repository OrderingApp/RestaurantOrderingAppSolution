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
    Console.WriteLine("       JSON files are resolved from AppContext.BaseDirectory/Seeder/SeedData/");
    Environment.Exit(0);
}

var positional = args.Where(a => !a.StartsWith("--")).ToArray();
var dbFile = positional.Length > 0 ? positional[0] : "seeder-test.db";
var services = new ServiceCollection();

services.AddDatabaseSeeders();

services.AddDbContext<RestaurantOrderingContext>(opts =>
{
    opts.UseSqlite($"Data Source={dbFile}");
});

var provider = services.BuildServiceProvider();

using var scope = provider.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
var db = scope.ServiceProvider.GetRequiredService<RestaurantOrderingContext>();
db.Database.EnsureCreated();

Console.WriteLine($"Ready to run DatabaseSeeder against SQLite file: {dbFile}");
Console.WriteLine($"JSON files resolved from: {Path.Combine(AppContext.BaseDirectory, "Seeder", "SeedData")}");
Console.WriteLine("Press Enter to proceed or Ctrl+C to abort.");
Console.ReadLine();

try
{
    await seeder.SeedAsync();

    Console.WriteLine("\nSeed run summary:");
    Console.WriteLine($"  MenuCategories     : {await db.MenuCategories.CountAsync()}");
    Console.WriteLine($"  SubCategories      : {await db.SubCategories.CountAsync()}");
    Console.WriteLine($"  IngredientCategories: {await db.IngredientCategories.CountAsync()}");
    Console.WriteLine($"  Tags               : {await db.Tags.CountAsync()}");
    Console.WriteLine($"  Allergens          : {await db.Allergens.CountAsync()}");
    Console.WriteLine($"  Areas              : {await db.Areas.CountAsync()}");
    Console.WriteLine($"  Ingredients        : {await db.Ingredients.CountAsync()}");
    Console.WriteLine($"  Tables             : {await db.Tables.CountAsync()}");
    Console.WriteLine($"  MenuItems          : {await db.MenuItems.CountAsync()}");
    Console.WriteLine($"  IngredientTagRels  : {await db.IngredientTagRels.CountAsync()}");
    Console.WriteLine($"  IngredientAllergenRels: {await db.IngredientAllergenRels.CountAsync()}");
    Console.WriteLine($"  MenuItemIngredientRels: {await db.MenuItemIngredientRels.CountAsync()}");
}
catch (Exception ex)
{
    Console.WriteLine($"Seeding failed: {ex.Message}");
    if (ex.InnerException != null)
        Console.WriteLine($"  Inner: {ex.InnerException.Message}");
}
