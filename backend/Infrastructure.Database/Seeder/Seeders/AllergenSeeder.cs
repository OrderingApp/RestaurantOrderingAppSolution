using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class AllergenSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public AllergenSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Allergens.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "allergens.json");
        var models = await _reader.ReadAsync<AllergenSeedModel>(path);
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
