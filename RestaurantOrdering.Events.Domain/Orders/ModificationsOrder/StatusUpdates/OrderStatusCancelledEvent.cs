namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.StatusUpdates;

public class OrderStatusCancelledEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderStatusCancelledEvent);
}