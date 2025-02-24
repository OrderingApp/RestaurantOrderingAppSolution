namespace Domain;

public class Ingredient
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }

    public bool CanBeUsedAsExtra { get; set; }
    public bool IsDeleted { get; set; } = false;

    public List<MenuItemIngredientRel> MenuItemIngredientRels { get; set; } = new();
    public List<IngredientTagRel> IngredientTagRels { get; set; } = new();
}