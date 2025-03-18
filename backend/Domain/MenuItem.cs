namespace Domain;

public class MenuItem
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int SequenceNumber { get; set; }

    public Guid? MenuCategoryId { get; set; }
    public MenuCategory? MenuCategory { get; set; } = null!;

    public Guid? SubCategoryId { get; set; }
    public SubCategory? SubCategory { get; set; } = null!;

    public List<MenuItemIngredientRel> MenuItemIngredientRels { get; set; } = new();
}