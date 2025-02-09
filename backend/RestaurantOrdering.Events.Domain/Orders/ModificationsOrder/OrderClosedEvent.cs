using Domain;

namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

public class OrderClosedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public List<OrderItem> OrderItems { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
    public override string GetEventType() => nameof(OrderClosedEvent);
}
