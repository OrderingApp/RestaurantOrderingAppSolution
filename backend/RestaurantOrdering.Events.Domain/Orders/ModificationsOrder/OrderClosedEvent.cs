using Domain;

namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

public class OrderClosedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public OrderStatus From { get; set; }
    public OrderStatus To { get; set; }

    public override string GetEventType() => nameof(OrderClosedEvent);
}
