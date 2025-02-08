namespace RestaurantOrdering.Events.Domain.Orders.DeleteOrder;

public class OrderDeletedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderDeletedEvent);
}
