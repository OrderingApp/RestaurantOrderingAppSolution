using Domain;

namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemStatusUpdatedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }
    public OrderItemStatus From { get; set; }
    public OrderItemStatus To { get; set; }

    public override string GetEventType() => nameof(OrderItemStatusUpdatedEvent);
}
