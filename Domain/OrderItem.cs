namespace Domain;

public class OrderItem
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? SpecialInstructions { get; set; }

    public List<OrderItemIngredient> OrderItemIngredients { get; set; } = new List<OrderItemIngredient>();

    public decimal Discount { get; set; } = 0;

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
}