namespace Application.Dtos.MenuItems;

public class MenuItemDetailedDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public List<MenuItemIngredientReadDto> BaseIngredients { get; set; } = new();
}
