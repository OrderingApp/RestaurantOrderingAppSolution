namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

public class OrderSplitEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderSplitEvent);
}
