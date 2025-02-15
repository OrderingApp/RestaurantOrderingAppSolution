namespace Domain;

public class OrderItem
{
    public Guid Id { get; set; }
    public required decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string? SpecialInstructions { get; set; }

    public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;

    public List<OrderItemIngredient> Ingredients { get; set; } = new();

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public Guid MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
}

public class OrderItemIngredient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
}

public enum OrderItemStatus
{
    Pending,
    Served,
    Cancelled
}