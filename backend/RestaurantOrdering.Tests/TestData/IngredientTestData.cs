using Application.Dtos.Ingredients;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class IngredientTestData
{
    public const string DefaultIngredientName = "Cheese";
    public const string UpdatedIngredientName = "Updated Cheese";

    public const string CheeseName = "Cheese";
    public const string TomatoName = "Tomato";
    public const string BaconName = "Bacon";

    public static Ingredient CreateIngredient(
        string name,
        decimal price,
        bool canBeUsedAsExtra,
        bool isDeleted = false)
    {
        return new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            CanBeUsedAsExtra = canBeUsedAsExtra,
            IsDeleted = isDeleted
        };
    }

    public static List<Ingredient> CreateDefaultIngredients() =>
    [
        CreateIngredient(CheeseName, 1.5m, true),
        CreateIngredient(TomatoName, 1m, true),
        CreateIngredient(BaconName, 2m, false)
    ];

    public static Ingredient CreateValidIngredient(Guid? id = null, string? name = null, decimal price = 1.0m)
    {
        return new Ingredient
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultIngredientName,
            Price = price,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            IngredientTagRels = new(),
            MenuItemIngredientRels = new()
        };
    }

    public static Ingredient CreateTaggedIngredient(string name, decimal price, Tag tag) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            IngredientTagRels = new List<IngredientTagRel>
            {
                new IngredientTagRel { Tag = tag, TagId = tag.Id }
            }
        };

    public static Ingredient CreateUntaggedIngredient(string name, decimal price) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            IngredientTagRels = new List<IngredientTagRel>()
        };

    public static IngredientCreateDto CreateCreateDto(string name = DefaultIngredientName, decimal price = 1.0m)
        => new() { Name = name, Price = price };

    public static IngredientUpdateDto CreateUpdateDto(string name = UpdatedIngredientName, decimal price = 2.0m)
        => new() { Name = name, Price = price };

    public static IngredientReadDto CreateReadDto(Guid id, string name = DefaultIngredientName, decimal price = 1.0m)
        => new() { Id = id, Name = name, Price = price };
}
