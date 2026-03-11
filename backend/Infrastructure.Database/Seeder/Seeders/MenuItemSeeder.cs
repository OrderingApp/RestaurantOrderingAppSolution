using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class MenuItemSeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public MenuItemSeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.MenuItems.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "menu-items.json");
        var models = await _reader.ReadAsync<MenuItemSeedModel>(path);
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
