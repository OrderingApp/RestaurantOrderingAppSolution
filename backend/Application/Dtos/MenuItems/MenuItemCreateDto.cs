namespace Application.Dtos.MenuItems;

public class MenuItemCreateDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public Guid MenuCategoryId { get; set; }
    public List<Guid> TagIds { get; set; } = new List<Guid>();
}
