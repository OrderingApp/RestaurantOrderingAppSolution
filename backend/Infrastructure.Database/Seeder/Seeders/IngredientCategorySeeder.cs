using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientCategorySeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public IngredientCategorySeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.IngredientCategories.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<IngredientCategorySeedModel>("ingredient-categories.json");
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
