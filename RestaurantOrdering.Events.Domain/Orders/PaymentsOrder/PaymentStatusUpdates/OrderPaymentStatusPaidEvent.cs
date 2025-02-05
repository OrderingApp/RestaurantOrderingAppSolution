namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder.PaymentStatusUpdates
{
    public class OrderPaymentStatusPaidEvent : BaseEvent
    {
        public Guid OrderId { get; set; }

        public override string GetEventType() => nameof(OrderPaymentStatusPaidEvent);
    }
}
