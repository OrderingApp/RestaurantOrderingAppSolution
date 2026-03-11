using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class TableSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public TableSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Tables.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "tables.json");
        var models = await _reader.ReadAsync<TableSeedModel>(path);
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
