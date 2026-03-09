using Application.Dtos.Allergens;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class AllergenTestData
{
    public const string DefaultAllergenName = "Gluten";
    public const string UpdatedAllergenName = "Updated Gluten";

    public const string GlutenName = "Gluten";
    public const string NutsName = "Nuts";
    public const string DairyName = "Dairy";

    public static Allergen CreateAllergen(string name) =>
        new() { Id = Guid.NewGuid(), Name = name };

    public static Allergen CreateValidAllergen(
        Guid? id = null,
        string? name = null,
        bool isUsed = true,
        bool isDeleted = false
    )
    {
        return new Allergen
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultAllergenName,
            IsUsed = isUsed,
            IsDeleted = isDeleted,
            IngredientAllergenRels = new(),
        };
    }

    public static List<Allergen> CreateDefaultAllergens() =>
        new()
        {
            CreateValidAllergen(name: GlutenName),
            CreateValidAllergen(name: NutsName),
        };

    public static AllergenCreateDto CreateCreateDto(string name = DefaultAllergenName) =>
        new() { Name = name };

    public static AllergenUpdateDto CreateUpdateDto(
        string name = UpdatedAllergenName,
        bool isUsed = true
    ) => new() { Name = name, IsUsed = isUsed };

    public static AllergenReadDto CreateReadDto(Guid id, string name = DefaultAllergenName) =>
        new() { Id = id, Name = name };
}
