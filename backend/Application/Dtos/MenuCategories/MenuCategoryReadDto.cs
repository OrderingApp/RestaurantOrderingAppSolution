using Application.Dtos.MenuItems;

namespace Application.Dtos.MenuCategories;

public class MenuCategoryReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public List<MenuItemReadDto> MenuItems { get; set; } = new List<MenuItemReadDto>();
}
