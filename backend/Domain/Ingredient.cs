namespace Domain;

public class Ingredient
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }

    public bool CanBeUsedAsExtra { get; set; }
    public bool IsDeleted { get; set; } = false;

    public Guid? CategoryId { get; set; }
    public IngredientCategory? Category { get; set; }

    public List<MenuItemIngredientRel> MenuItemIngredientRels { get; set; } = new(); // check if this is needed
    public List<IngredientTagRel> IngredientTagRels { get; set; } = new();
}
