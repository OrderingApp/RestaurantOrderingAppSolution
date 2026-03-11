using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class AreaSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public AreaSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Areas.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "areas.json");
        var models = await _reader.ReadAsync<AreaSeedModel>(path);
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new Area
        {
            Id = m.Id,
            Name = m.Name,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted,
            SequenceNumber = m.SequenceNumber
        }).ToList();

        await _context.Areas.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
