using Domain;

namespace RestaurantOrdering.Events.Domain.Payments;

public class PaymentStatusChangedEvent : BaseEvent
{
    public Guid PaymentId { get; set; }
    public PaymentStatus From { get; set; }
    public PaymentStatus To { get; set; }

    public override string GetEventType() => nameof(PaymentStatusChangedEvent);
}
