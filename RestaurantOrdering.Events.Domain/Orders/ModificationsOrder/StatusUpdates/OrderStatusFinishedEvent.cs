namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.StatusUpdates;

public class OrderStatusFinishedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderStatusFinishedEvent);
}
