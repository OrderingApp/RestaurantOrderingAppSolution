namespace Domain;

public class OrderItemIngredient
{
    public Guid OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;

    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}
