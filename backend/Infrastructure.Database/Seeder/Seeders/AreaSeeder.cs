using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class AreaSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public AreaSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Areas.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<AreaSeedModel>("areas.json");
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
