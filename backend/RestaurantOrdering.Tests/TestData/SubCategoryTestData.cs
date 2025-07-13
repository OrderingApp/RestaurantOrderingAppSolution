using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class SubCategoryTestData
{
    public const string DefaultSubCategoryName = "Subs";
    public const string UpdatedSubCategoryName = "Updated Subs";

    public static SubCategory CreateValidSubCategory(Guid? id = null, string? name = null, int sequence = 1)
    {
        return new SubCategory
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultSubCategoryName,
            SequenceNumber = sequence,
            MenuItems = new()
        };
    }

    public static SubCategory CreateSubCategory(string name = "Subs", int sequence = 1) =>
    new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        SequenceNumber = sequence
    };


    public static List<SubCategory> CreateDefaultSubCategories() =>
    [
        CreateValidSubCategory(name: "Subs", sequence: 1),
        CreateValidSubCategory(name: "Wraps", sequence: 2)
    ];
}
