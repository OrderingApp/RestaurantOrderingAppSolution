namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemAddedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public List<Guid> OrderItemIds { get; set; } = new();

    public override string GetEventType() => nameof(OrderItemAddedEvent);
}
