using Application.Dtos.Areas;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class AreaTestData
{
    public const string UniqueAreaName = "Unique Area";
    public const string ExistingAreaName = "Main Hall";
    public const string UpdatedAreaName = "Updated Name";
    public const string TemporaryAreaName = "Temporary Area";

    public static Area CreateValidArea(Guid? id = null, string? name = null)
    {
        return new Area
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? UniqueAreaName,
            IsUsed = true,
            IsDeleted = false,
            Tables = []
        };
    }

    public static List<Area> CreateAreaList() =>
    [
        CreateValidArea(name: ExistingAreaName),
        CreateValidArea(name: "Terrace"),
        CreateValidArea(name: "VIP Room")
    ];

    public static class AreaDtoTestData
    {
        public static AreaCreateDto CreateAreaDto(string name = UniqueAreaName) =>
            new() { Name = name };

        public static AreaUpdateDto UpdateAreaDto(string name = UpdatedAreaName) =>
            new() { Name = name };
    }
}
