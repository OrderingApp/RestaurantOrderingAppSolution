namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

public class OrderSplitBillEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderSplitBillEvent);
}
