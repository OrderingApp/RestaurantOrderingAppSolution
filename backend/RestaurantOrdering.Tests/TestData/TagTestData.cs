using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class TagTestData
{
    public const string VeganTag = "Vegan";
    public const string SpicyTag = "Spicy";
    public const string MeatTag = "Meat";

    public static Tag CreateTag(string name) => new() { Id = Guid.NewGuid(), Name = name };
}
