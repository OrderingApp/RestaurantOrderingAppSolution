using Application.Dtos.Tags;

namespace Application.Dtos.SubCategories;

public class SubCategoryReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalItemsInSubCategory { get; set; }
    public List<TagReadDto> Tags { get; set; } = new();
}
