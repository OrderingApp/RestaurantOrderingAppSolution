using Application.Dtos.MenuItems;
using Application.Dtos.SubCategories;
using Application.Dtos.Tags;

namespace Application.Dtos.MenuCategories;

public class MenuCategoryHierarchyReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalItems { get; set; }
    public List<TagReadDto> Tags { get; set; } = new();
    public List<SubCategoryReadDto> SubCategories { get; set; } = new();
    public List<MenuItemReadDto> MenuItems { get; set; } = new();
}
