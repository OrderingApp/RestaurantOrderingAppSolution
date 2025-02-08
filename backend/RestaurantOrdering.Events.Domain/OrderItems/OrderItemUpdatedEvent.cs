namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemUpdatedEvent : BaseEvent
{
    public Guid OrderItemId { get; set; }

    public override string GetEventType() => nameof(OrderItemUpdatedEvent);
}
