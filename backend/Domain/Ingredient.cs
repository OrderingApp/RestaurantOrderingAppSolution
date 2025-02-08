namespace Domain;

public class Ingredient
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }

    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<OrderItemIngredient> OrderItemIngredients { get; set; } = new List<OrderItemIngredient>();
    public required IngredientType IngredientType { get; set; }
}

public enum IngredientType
{
    Cheese,
    Vegetables,
    Meat,
    Other
}