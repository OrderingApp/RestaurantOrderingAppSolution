namespace Application.Dtos.MenuItems;

public class MenuItemReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public List<MenuItemIngredientReadDto> Ingredients { get; set; } = new();
    public Guid? SubCategoryId { get; set; }
}
