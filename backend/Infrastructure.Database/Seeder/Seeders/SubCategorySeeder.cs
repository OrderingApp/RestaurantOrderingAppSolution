using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Infrastructure.Database.Seeder.SeedModels;

namespace Infrastructure.Database.Seeder.Seeders;

public class SubCategorySeeder : ISeeder
{
    private readonly RestaurantOrderingContext _context;
    private readonly SeedDataReader _reader;

    public SubCategorySeeder(RestaurantOrderingContext context, SeedDataReader reader)
    {
        _context = context;
        _reader = reader;
    }

    public async Task SeedAsync()
    {
        if (_context.SubCategories.Any())
            return;

        var models = await _reader.ReadByFileNameAsync<SubCategorySeedModel>("sub-categories.json");
        if (models == null || !models.Any())
            return;

        var entities = models.Select(m => new SubCategory
        {
            Id = m.Id,
            Name = m.Name,
            IsUsed = m.IsUsed,
            IsDeleted = m.IsDeleted,
            MenuCategoryId = m.MenuCategoryId,
            SequenceNumber = m.SequenceNumber
        }).ToList();

        await _context.SubCategories.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}
