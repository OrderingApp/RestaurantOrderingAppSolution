namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

public class OrderItemsMovedEvent : BaseEvent
{
    public Guid SourceOrderId { get; set; }
    public Guid TargetOrderId { get; set; }
    public List<Guid> MovedItemIds { get; set; } = new();

    public override string GetEventType() => nameof(OrderItemsMovedEvent);
}
