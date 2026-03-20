using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class MenuItemSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public MenuItemSeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.MenuItems.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<MenuItemSeedModel>("menu-items.json");
        if (models == null || models.Count == 0)
            return;

        var entities = models.Select(m => new MenuItem
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            Price = m.Price,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted,
            MenuCategoryId = m.MenuCategoryId,
            SubCategoryId = m.SubCategoryId,
            SequenceNumber = m.SequenceNumber
        }).ToList();

        await _context.MenuItems.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
