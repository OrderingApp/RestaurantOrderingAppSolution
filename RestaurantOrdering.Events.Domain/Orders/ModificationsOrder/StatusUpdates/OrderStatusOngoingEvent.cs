namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.StatusUpdates;

public class OrderStatusOngoingEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderStatusOngoingEvent);
}
