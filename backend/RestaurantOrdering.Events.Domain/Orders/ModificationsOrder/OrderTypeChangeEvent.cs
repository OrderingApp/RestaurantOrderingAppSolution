using Domain;

namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

public class OrderTypeChangeEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public OrderType From { get; set; }
    public OrderType To { get; set; }

    public override string GetEventType() => nameof(OrderTypeChangeEvent);
}