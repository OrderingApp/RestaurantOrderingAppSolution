using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class TableSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public TableSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Tables.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<TableSeedModel>("tables.json");
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new Table
        {
            Id = m.Id,
            Name = m.Name,
            Capacity = m.Capacity,
            SequenceNumber = m.SequenceNumber,
            IsPrepared = m.IsPrepared,
            ActiveSince = m.ActiveSince,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted,
            Status = m.Status,
            AreaId = m.AreaId
        }).ToList();

        await _context.Tables.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
