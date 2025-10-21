// IntegrationTests/TestData/TestData.cs
using Infrastructure.Database;

public sealed class TestData
{
    private readonly RestaurantOrderingContext _db;
    public TestData(RestaurantOrderingContext db) => _db = db;

    public async Task<Guid> AreaAsync(string name = "Main Hall")
    {
        var id = Guid.NewGuid();
        _db.Areas.Add(new Domain.Area { Id = id, Name = name, IsUsed = true, IsDeleted = false });
        await _db.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> TableAsync(Guid areaId, string name = "P1", int capacity = 4)
    {
        var id = Guid.NewGuid();
        _db.Tables.Add(new Domain.Table
        {
            Id = id,
            Name = name,
            Capacity = capacity,
            AreaId = areaId,
            IsUsed = true,
            IsDeleted = false,
            Status = Domain.TableStatus.Available
        });
        await _db.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> MenuCategoryAsync(string name = "Pizza")
    {
        var id = Guid.NewGuid();
        _db.MenuCategories.Add(new Domain.MenuCategory { Id = id, Name = name });
        await _db.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> IngredientAsync(string name, decimal price, bool canBeExtra = true)
    {
        var id = Guid.NewGuid();
        _db.Ingredients.Add(new Domain.Ingredient { Id = id, Name = name, Price = price, CanBeUsedAsExtra = canBeExtra });
        await _db.SaveChangesAsync();
        return id;
    }

    public async Task<Guid> MenuItemWithBaseIngredientsAsync(string name, decimal price, Guid categoryId, params Guid[] baseIngredientIds)
    {
        var itemId = Guid.NewGuid();
        _db.MenuItems.Add(new Domain.MenuItem { Id = itemId, Name = name, Price = price, MenuCategoryId = categoryId });
        foreach (var ingId in baseIngredientIds)
            _db.MenuItemIngredientRels.Add(new Domain.MenuItemIngredientRel { MenuItemId = itemId, IngredientId = ingId });
        await _db.SaveChangesAsync();
        return itemId;
    }
}
