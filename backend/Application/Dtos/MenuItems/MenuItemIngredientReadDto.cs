namespace Application.Dtos.MenuItems;

public class MenuItemIngredientReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TagId { get; set; }
}