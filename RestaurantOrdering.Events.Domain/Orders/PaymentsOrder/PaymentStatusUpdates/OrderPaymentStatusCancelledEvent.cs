namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder.PaymentStatusUpdates;

public class OrderPaymentStatusCancelledEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderPaymentStatusDefferedPaymentEvent);
}
