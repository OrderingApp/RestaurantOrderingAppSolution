namespace RestaurantOrdering.Events.Domain.Orders.CreatingOrder;

public class DineInOrderCreatedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(DineInOrderCreatedEvent);
}
