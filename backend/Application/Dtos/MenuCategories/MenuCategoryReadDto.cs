using Application.Dtos.SubCategories;

namespace Application.Dtos.MenuCategories;

public class MenuCategoryReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalItems { get; set; }
    public List<SubCategoryReadDto> SubCategories { get; set; } = new();
}