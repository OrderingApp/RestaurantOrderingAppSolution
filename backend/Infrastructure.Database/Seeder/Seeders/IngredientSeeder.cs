using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class IngredientSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public IngredientSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Ingredients.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<IngredientSeedModel>("ingredients.json");
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
