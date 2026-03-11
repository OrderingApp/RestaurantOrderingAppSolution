using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public IngredientSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Ingredients.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "ingredients.json");
        var models = await _reader.ReadAsync<IngredientSeedModel>(path);
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new Ingredient
        {
            Id = m.Id,
            Name = m.Name,
            Price = m.Price,
            CanBeUsedAsExtra = m.CanBeUsedAsExtra,
            IsDeleted = m.IsDeleted,
            CategoryId = m.CategoryId
        }).ToList();

        await _context.Ingredients.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
