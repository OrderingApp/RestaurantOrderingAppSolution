using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientCategorySeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public IngredientCategorySeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.IngredientCategories.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "ingredient-categories.json");
        var models = await _reader.ReadAsync<IngredientCategorySeedModel>(path);
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new IngredientCategory
        {
            Id = m.Id,
            Name = m.Name,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted
        }).ToList();

        await _context.IngredientCategories.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
