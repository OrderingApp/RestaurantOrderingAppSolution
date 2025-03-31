using Domain;

namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

public class OrderClosedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public List<OrderClosedEventOrderItem> OrderItems { get; set; } = new();
    public List<OrderClosedEventPayment> Payments { get; set; } = new();
    public override string GetEventType() => nameof(OrderClosedEvent);
}


public class OrderClosedEventOrderItem
{
    public Guid MenuItemId { get; set; }
    public List<OrderItemIngredient> ExtraIngredients { get; set; } = new();
    public List<OrderItemIngredient> RemovedIngredients { get; set; } = new();
    public required decimal Price { get; set; }
}

public class OrderClosedEventPayment
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}