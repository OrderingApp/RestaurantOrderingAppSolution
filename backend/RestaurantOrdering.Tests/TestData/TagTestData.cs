using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class TagTestData
{
    public const string VeganTag = "Vegan";
    public const string SpicyTag = "Spicy";
    public const string MeatTag = "Meat";


    public const string DefaultTagName = "Spicy";
    public const string UpdatedTagName = "Mild";

    public static Tag CreateTag(string name) => new() { Id = Guid.NewGuid(), Name = name };

    public static Tag CreateValidTag(Guid? id = null, string? name = null, bool isUsed = true, bool isDeleted = false)
    {
        return new Tag
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultTagName,
            IsUsed = isUsed,
            IsDeleted = isDeleted,
            IngredientTagRels = new()
        };
    }

    public static List<Tag> CreateDefaultTags() => new()
    {
        CreateValidTag(name: "Vegan"),
        CreateValidTag(name: "Gluten-Free")
    };
}
