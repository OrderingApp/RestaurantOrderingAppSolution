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

    public const int DefaultEuNumber = 1;
    public const int UpdatedEuNumber = 8;

    public static Allergen CreateAllergen(string name, int? euNumber = null) =>
        new() { Id = Guid.NewGuid(), Name = name, EuNumber = euNumber };

    public static Allergen CreateValidAllergen(
        Guid? id = null,
        string? name = null,
        int? euNumber = null,
        bool isUsed = true,
        bool isDeleted = false
    )
    {
        return new Allergen
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultAllergenName,
            EuNumber = euNumber,
            IsUsed = isUsed,
            IsDeleted = isDeleted,
            IngredientAllergenRels = new(),
        };
    }

    public static List<Allergen> CreateDefaultAllergens() =>
        new()
        {
            CreateValidAllergen(name: GlutenName, euNumber: 1),
            CreateValidAllergen(name: NutsName, euNumber: 8),
        };

    public static AllergenCreateDto CreateCreateDto(
        string name = DefaultAllergenName,
        int? euNumber = null
    ) => new() { Name = name, EuNumber = euNumber };

    public static AllergenUpdateDto CreateUpdateDto(
        string name = UpdatedAllergenName,
        int? euNumber = null,
        bool isUsed = true
    ) => new() { Name = name, EuNumber = euNumber, IsUsed = isUsed };

    public static AllergenReadDto CreateReadDto(
        Guid id,
        string name = DefaultAllergenName,
        int? euNumber = null
    ) => new() { Id = id, Name = name, EuNumber = euNumber };
}
