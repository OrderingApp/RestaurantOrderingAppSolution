namespace Application.Dtos.MenuItems;

public class MenuItemReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }

    public int SequenceNumber { get; set; }
    public Guid? SubCategoryId { get; set; }
    public Guid? MenuCategoryId { get; set; }

    public List<MenuItemIngredientWithTagsDto> Ingredients { get; set; } = new();
}
