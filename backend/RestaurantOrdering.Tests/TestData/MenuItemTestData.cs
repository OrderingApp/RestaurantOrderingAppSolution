using Application.Dtos.MenuItems;
using Domain;

public static class MenuItemTestData
{
    public const string DefaultMenuItemName = "Pizza";
    public const string DefaultDescription = "Cheesy goodness";
    public const decimal DefaultPrice = 12.5m;

    public static MenuItem CreateMenuItem(
        Guid? id = null,
        string? name = null,
        decimal price = DefaultPrice,
        bool isUsed = true,
        bool isDeleted = false,
        int sequenceNumber = 1,
        Guid? menuCategoryId = null,
        Guid? subCategoryId = null
    )
    {
        return new MenuItem
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultMenuItemName,
            Price = price,
            IsUsed = isUsed,
            IsDeleted = isDeleted,
            SequenceNumber = sequenceNumber,
            MenuCategoryId = menuCategoryId,
            SubCategoryId = subCategoryId,
            MenuItemIngredientRels = new List<MenuItemIngredientRel>()
        };
    }

    public static MenuItemCreateDto CreateCreateDto(
        string name = DefaultMenuItemName,
        string description = DefaultDescription,
        decimal price = DefaultPrice,
        Guid? menuCategoryId = null,
        Guid? subCategoryId = null,
        List<Guid>? ingredientIds = null
    ) =>
        new()
        {
            Name = name,
            Description = description,
            Price = price,
            MenuCategoryId = menuCategoryId ?? Guid.NewGuid(),
            SubCategoryId = subCategoryId,
            IngredientIds = ingredientIds ?? new()
        };

    public static MenuItemReadDto CreateReadDto(
        MenuItem menuItem,
        List<MenuItemIngredientReadDto>? ingredients = null
    ) =>
        new()
        {
            Id = menuItem.Id,
            Name = menuItem.Name,
            Description = menuItem.Description!,
            Price = menuItem.Price,
            SequenceNumber = menuItem.SequenceNumber,
            MenuCategoryId = menuItem.MenuCategoryId,
            SubCategoryId = menuItem.SubCategoryId,
            Ingredients = ingredients ?? []
        };
}
