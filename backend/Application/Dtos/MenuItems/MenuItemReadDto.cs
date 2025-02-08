using Application.Dtos.MenuItemTags;

namespace Application.Dtos.MenuItems;

public class MenuItemReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public List<MenuItemTagReadDto> Tags { get; set; } = new List<MenuItemTagReadDto>();
}
