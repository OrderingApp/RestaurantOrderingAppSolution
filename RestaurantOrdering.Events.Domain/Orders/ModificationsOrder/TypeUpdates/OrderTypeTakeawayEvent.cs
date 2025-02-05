namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.TypeUpdates;

public class OrderTypeTakeawayEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderTypeTakeawayEvent);
}
