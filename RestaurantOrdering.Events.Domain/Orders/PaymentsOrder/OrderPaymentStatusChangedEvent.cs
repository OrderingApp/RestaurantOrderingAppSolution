using Domain;

namespace RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

public class OrderPaymentStatusChangedEvent : BaseEvent
{
    public Guid OrderId { get; set; }
    public OrderStatus From { get; set; }
    public OrderStatus To { get; set; }

    public override string GetEventType() => nameof(OrderPaymentStatusChangedEvent);
}
