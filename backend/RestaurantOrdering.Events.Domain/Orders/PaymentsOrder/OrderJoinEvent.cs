namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

public class OrderJoinEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderJoinEvent);
}