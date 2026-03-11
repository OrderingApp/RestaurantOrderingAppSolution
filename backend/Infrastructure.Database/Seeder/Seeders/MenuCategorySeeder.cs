using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Database.Seeder.Seeders;

public class MenuCategorySeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly SeedDataReader _reader;

    public MenuCategorySeeder(RestaurantOrderingContext context, IWebHostEnvironment env, SeedDataReader reader)
    {
        _context = context;
        _env = env;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.MenuCategories.Any())
            return;

        var path = Path.Combine(_env.ContentRootPath, "Seeder", "SeedData", "menu-categories.json");
        var models = await _reader.ReadAsync<MenuCategorySeedModel>(path);
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
