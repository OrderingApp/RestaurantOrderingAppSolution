namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.TypeUpdates;

public class OrderTypeDeliveryEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderTypeDeliveryEvent);
}
