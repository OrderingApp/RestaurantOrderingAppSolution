namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.TypeUpdates;

public class OrderTypeDineInEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderTypeDineInEvent);
}
