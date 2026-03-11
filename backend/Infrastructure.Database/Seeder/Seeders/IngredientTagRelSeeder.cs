using System.IO;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientTagRelSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public IngredientTagRelSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        // keep legacy behavior guard
        if (_context.IngredientTagRels.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "ingredient-tag-rels.json");
        var models = await _reader.ReadAsync<IngredientTagRelSeedModel>(path);
        if (models == null || models.Count == 0)
            return;

        // load existing FK sets for validation
        var ingredientIds = new HashSet<Guid>(await _context.Ingredients.Select(i => i.Id).ToListAsync());
        var tagIds = new HashSet<Guid>(await _context.Tags.Select(t => t.Id).ToListAsync());
        var existingList = await _context.IngredientTagRels.Select(r => new { r.IngredientId, r.TagId }).ToListAsync();
        var existing = new HashSet<(Guid, Guid)>(existingList.Select(x => (x.IngredientId, x.TagId)));

        var toAdd = new List<IngredientTagRel>();
        var skipped = 0;
        var warnings = new List<string>();

        foreach (var m in models)
        {
            if (!ingredientIds.Contains(m.IngredientId))
            {
                skipped++;
                warnings.Add($"Skipped relation IngredientTagRel: missing Ingredient {m.IngredientId}");
                continue;
            }

            if (!tagIds.Contains(m.TagId))
            {
                skipped++;
                warnings.Add($"Skipped relation IngredientTagRel: missing Tag {m.TagId}");
                continue;
            }

            var key = (m.IngredientId, m.TagId);
            if (existing.Contains(key))
            {
                // already present
                continue;
            }

            toAdd.Add(new IngredientTagRel { IngredientId = m.IngredientId, TagId = m.TagId });
            existing.Add(key);
        }

        if (toAdd.Count > 0)
        {
            await _context.IngredientTagRels.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        // report to console for dev-runner visibility
        Console.WriteLine($"IngredientTagRelSeeder: added {toAdd.Count}, skipped {skipped}");
        foreach (var w in warnings.Take(10)) Console.WriteLine(w);
    }

    // Dev-only: run relations with report
    public async Task<RelationalSeedReport> SeedRelationsAsync()
    {
        var report = new RelationalSeedReport();
        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "ingredient-tag-rels.json");
        var models = await _reader.ReadAsync<IngredientTagRelSeedModel>(path);
        if (models == null || models.Count == 0)
            return report;

        var ingredientIds = new HashSet<Guid>(await _context.Ingredients.Select(i => i.Id).ToListAsync());
        var tagIds = new HashSet<Guid>(await _context.Tags.Select(t => t.Id).ToListAsync());
        var existingList = await _context.IngredientTagRels.Select(r => new { r.IngredientId, r.TagId }).ToListAsync();
        var existing = new HashSet<(Guid, Guid)>(existingList.Select(x => (x.IngredientId, x.TagId)));

        var toAdd = new List<IngredientTagRel>();
        foreach (var m in models)
        {
            if (!ingredientIds.Contains(m.IngredientId))
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped relation IngredientTagRel: missing Ingredient {m.IngredientId}");
                continue;
            }

            if (!tagIds.Contains(m.TagId))
            {
                report.SkippedCount++;
                report.Warnings.Add($"Skipped relation IngredientTagRel: missing Tag {m.TagId}");
                continue;
            }

            var key = (m.IngredientId, m.TagId);
            if (existing.Contains(key))
                continue;

            toAdd.Add(new IngredientTagRel { IngredientId = m.IngredientId, TagId = m.TagId });
            existing.Add(key);
        }

        if (toAdd.Count > 0)
        {
            await _context.IngredientTagRels.AddRangeAsync(toAdd);
            await _context.SaveChangesAsync();
        }

        report.AddedCount = toAdd.Count;
        return report;
    }
}
