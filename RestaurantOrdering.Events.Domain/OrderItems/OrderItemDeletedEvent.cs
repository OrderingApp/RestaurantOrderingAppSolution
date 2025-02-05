namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemDeletedEvent : BaseEvent
{
    public Guid OrderItemId { get; set; }

    public override string GetEventType() => nameof(OrderItemDeletedEvent);
}
