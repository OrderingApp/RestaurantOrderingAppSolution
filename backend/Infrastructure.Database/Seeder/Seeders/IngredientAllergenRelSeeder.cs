using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientAllergenRelSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public IngredientAllergenRelSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.IngredientAllergenRels.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "ingredient-allergen-rels.json");
        var models = await _reader.ReadAsync<IngredientAllergenRelSeedModel>(path);
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new IngredientAllergenRel
        {
            IngredientId = m.IngredientId,
            AllergenId = m.AllergenId
        }).ToList();

        await _context.IngredientAllergenRels.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
