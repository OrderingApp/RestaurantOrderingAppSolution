using Application.Dtos.MenuCategories;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class MenuCategoryTestData
{
    public const string DefaultCategoryName = "Starters";
    public const string UpdatedCategoryName = "Updated Starters";

    public static MenuCategory CreateValidCategory(Guid? id = null, string? name = null, bool isDeleted = false, bool isUsed = true) =>
        new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? DefaultCategoryName,
            IsDeleted = isDeleted,
            IsUsed = isUsed,
            SequenceNumber = 1,
            SubCategories = new(),
            MenuItems = new()
        };

    public static MenuCategoryCreateDto CreateCreateDto(string name = DefaultCategoryName) =>
        new() { Name = name };

    public static MenuCategoryUpdateDto CreateUpdateDto(string name = UpdatedCategoryName) =>
        new() { Name = name };

    public static MenuCategoryReadDto CreateReadDto(Guid id, string name = DefaultCategoryName) =>
        new() { Id = id, Name = name };
}
