using Application.Dtos.Ingredients;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class IngredientTestData
{
    public const string DefaultIngredientName = "Cheese";
    public const string UpdatedIngredientName = "Updated Cheese";

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

    public static IngredientCreateDto CreateCreateDto(string name = DefaultIngredientName, decimal price = 1.0m)
        => new() { Name = name, Price = price };

    public static IngredientUpdateDto CreateUpdateDto(string name = UpdatedIngredientName, decimal price = 2.0m)
        => new() { Name = name, Price = price };

    public static IngredientReadDto CreateReadDto(Guid id, string name = DefaultIngredientName, decimal price = 1.0m)
        => new() { Id = id, Name = name, Price = price };
}
