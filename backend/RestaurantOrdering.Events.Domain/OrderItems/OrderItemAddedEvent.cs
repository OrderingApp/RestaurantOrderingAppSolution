namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemAddedEvent : BaseEvent
{
    public Guid OrderItemId { get; set; }

    public override string GetEventType() => nameof(OrderItemAddedEvent);
}
