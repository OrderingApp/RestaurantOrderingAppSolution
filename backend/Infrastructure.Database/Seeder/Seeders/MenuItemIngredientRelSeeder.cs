using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class MenuItemIngredientRelSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public MenuItemIngredientRelSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.MenuItemIngredientRels.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<MenuItemIngredientRelSeedModel>("menu-item-ingredient-rels.json");
        if (models == null || models.Count == 0)
            return;

        var menuItemIds = new HashSet<Guid>(await _context.MenuItems.Select(mi => mi.Id).ToListAsync());
        var ingredientIds = new HashSet<Guid>(await _context.Ingredients.Select(i => i.Id).ToListAsync());
        var existingList = await _context.MenuItemIngredientRels.Select(r => new { r.MenuItemId, r.IngredientId }).ToListAsync();
        var existing = new HashSet<(Guid, Guid)>(existingList.Select(x => (x.MenuItemId, x.IngredientId)));

        var toAdd = new List<MenuItemIngredientRel>();
        var skipped = 0;
        var warnings = new List<string>();

        foreach (var m in models)
        {
            if (!menuItemIds.Contains(m.MenuItemId))
            {
                skipped++;
                warnings.Add($"Skipped relation MenuItemIngredientRel: missing MenuItem {m.MenuItemId}");
                continue;
            }

            if (!ingredientIds.Contains(m.IngredientId))
            {
                skipped++;
                warnings.Add($"Skipped relation MenuItemIngredientRel: missing Ingredient {m.IngredientId}");
                continue;
            }

            var key = (m.MenuItemId, m.IngredientId);
            if (existing.Contains(key))
                continue;

            toAdd.Add(new MenuItemIngredientRel { MenuItemId = m.MenuItemId, IngredientId = m.IngredientId });
            existing.Add(key);
        }

        if (toAdd.Count > 0)
        {
            await _context.MenuItemIngredientRels.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        Console.WriteLine($"MenuItemIngredientRelSeeder: added {toAdd.Count}, skipped {skipped}");
        foreach (var w in warnings.Take(10)) Console.WriteLine(w);
    }

    public async Task<RelationalSeedReport> SeedRelationsAsync()
    {
        var report = new RelationalSeedReport();
        var models = await _reader.ReadByFileNameAsync<MenuItemIngredientRelSeedModel>("menu-item-ingredient-rels.json");
        if (models == null || models.Count == 0)
            return report;

        var menuItemIds = new HashSet<Guid>(await _context.MenuItems.Select(mi => mi.Id).ToListAsync());
        var ingredientIds = new HashSet<Guid>(await _context.Ingredients.Select(i => i.Id).ToListAsync());
        var existingList = await _context.MenuItemIngredientRels.Select(r => new { r.MenuItemId, r.IngredientId }).ToListAsync();
        var existing = new HashSet<(Guid, Guid)>(existingList.Select(x => (x.MenuItemId, x.IngredientId)));

        var toAdd = new List<MenuItemIngredientRel>();
        foreach (var m in models)
        {
            if (!menuItemIds.Contains(m.MenuItemId))
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped relation MenuItemIngredientRel: missing MenuItem {m.MenuItemId}");
                continue;
            }

            if (!ingredientIds.Contains(m.IngredientId))
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped relation MenuItemIngredientRel: missing Ingredient {m.IngredientId}");
                continue;
            }

            var key = (m.MenuItemId, m.IngredientId);
            if (existing.Contains(key))
                continue;

            toAdd.Add(new MenuItemIngredientRel { MenuItemId = m.MenuItemId, IngredientId = m.IngredientId });
            existing.Add(key);
        }

        if (toAdd.Count > 0)
        {
            await _context.MenuItemIngredientRels.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        report.AddedCount = toAdd.Count;
        return report;
    }
}
