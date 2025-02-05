namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder.PaymentStatusUpdates;

public class OrderPaymentStatusPendingEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderPaymentStatusPendingEvent);
}
