namespace RestaurantOrdering.Events.Domain.Payments;

public class PaymentCreatedEvent : BaseEvent
{
    public Guid PaymentId { get; set; }

    public override string GetEventType() => nameof(PaymentCreatedEvent);
}
