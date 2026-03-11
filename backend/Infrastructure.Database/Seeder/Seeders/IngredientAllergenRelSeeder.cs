using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientAllergenRelSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public IngredientAllergenRelSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.IngredientAllergenRels.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<IngredientAllergenRelSeedModel>("ingredient-allergen-rels.json");
        if (models == null || models.Count == 0)
            return;

        var ingredientIds = new HashSet<Guid>(await _context.Ingredients.Select(i => i.Id).ToListAsync());
        var allergenIds = new HashSet<Guid>(await _context.Allergens.Select(a => a.Id).ToListAsync());
        var existingList = await _context.IngredientAllergenRels.Select(r => new { r.IngredientId, r.AllergenId }).ToListAsync();
        var existing = new HashSet<(Guid, Guid)>(existingList.Select(x => (x.IngredientId, x.AllergenId)));

        var toAdd = new List<IngredientAllergenRel>();
        var skipped = 0;
        var warnings = new List<string>();

        foreach (var m in models)
        {
            if (!ingredientIds.Contains(m.IngredientId))
            {
                skipped++;
                warnings.Add($"Skipped IngredientAllergenRel: missing Ingredient {m.IngredientId}");
                continue;
            }

            if (!allergenIds.Contains(m.AllergenId))
            {
                skipped++;
                warnings.Add($"Skipped IngredientAllergenRel: missing Allergen {m.AllergenId}");
                continue;
            }

            var key = (m.IngredientId, m.AllergenId);
            if (existing.Contains(key))
                continue;

            toAdd.Add(new IngredientAllergenRel { IngredientId = m.IngredientId, AllergenId = m.AllergenId });
            existing.Add(key);
        }

        if (toAdd.Count > 0)
        {
            await _context.IngredientAllergenRels.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        Console.WriteLine($"IngredientAllergenRelSeeder: added={toAdd.Count}, skipped={skipped}");
        foreach (var w in warnings.Take(10)) Console.WriteLine(w);
    }

    public async Task<RelationalSeedReport> SeedRelationsAsync()
    {
        var report = new RelationalSeedReport();
        var models = await _reader.ReadByFileNameAsync<IngredientAllergenRelSeedModel>("ingredient-allergen-rels.json");
        if (models == null || models.Count == 0)
            return report;

        var ingredientIds = new HashSet<Guid>(await _context.Ingredients.Select(i => i.Id).ToListAsync());
        var allergenIds = new HashSet<Guid>(await _context.Allergens.Select(a => a.Id).ToListAsync());
        var existingList = await _context.IngredientAllergenRels.Select(r => new { r.IngredientId, r.AllergenId }).ToListAsync();
        var existing = new HashSet<(Guid, Guid)>(existingList.Select(x => (x.IngredientId, x.AllergenId)));

        var toAdd = new List<IngredientAllergenRel>();
        foreach (var m in models)
        {
            if (!ingredientIds.Contains(m.IngredientId))
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped IngredientAllergenRel: missing Ingredient {m.IngredientId}");
                continue;
            }

            if (!allergenIds.Contains(m.AllergenId))
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped IngredientAllergenRel: missing Allergen {m.AllergenId}");
                continue;
            }

            var key = (m.IngredientId, m.AllergenId);
            if (existing.Contains(key))
                continue;

            toAdd.Add(new IngredientAllergenRel { IngredientId = m.IngredientId, AllergenId = m.AllergenId });
            existing.Add(key);
        }

        if (toAdd.Count > 0)
        {
            await _context.IngredientAllergenRels.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        report.AddedCount = toAdd.Count;
        return report;
    }
}
