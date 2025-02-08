namespace RestaurantOrdering.Events.Domain.Orders.CreatingOrder;

public class TakeawayOrderCreatedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(TakeawayOrderCreatedEvent);
}
