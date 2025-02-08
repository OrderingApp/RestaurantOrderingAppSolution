namespace Application.Dtos.MenuItems;

public class MenuItemUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public List<Guid> TagIds { get; set; } = new List<Guid>();
}
