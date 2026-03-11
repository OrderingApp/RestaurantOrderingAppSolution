using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class MenuCategorySeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public MenuCategorySeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.MenuCategories.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<MenuCategorySeedModel>("menu-categories.json");
        if (models == null || !models.Any())
            return;

        var entities = models.Select(m => new MenuCategory
        {
            Id = m.Id,
            Name = m.Name,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted,
            SequenceNumber = m.SequenceNumber
        }).ToList();

        await _context.MenuCategories.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
