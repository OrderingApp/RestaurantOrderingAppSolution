using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class TagSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public TagSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.Tags.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<TagSeedModel>("tags.json");
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new Tag
        {
            Id = m.Id,
            Name = m.Name,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted
        }).ToList();

        await _context.Tags.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
