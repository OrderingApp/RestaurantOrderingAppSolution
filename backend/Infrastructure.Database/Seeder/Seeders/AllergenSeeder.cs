using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class AllergenSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public AllergenSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Allergens.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<AllergenSeedModel>("allergens.json");
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new Allergen
        {
            Id = m.Id,
            Name = m.Name,
            EuNumber = m.EuNumber,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted
        }).ToList();

        await _context.Allergens.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
