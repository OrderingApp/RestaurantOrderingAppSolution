namespace RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

public class OrderTableChangedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderTableChangedEvent);
}
