namespace RestaurantOrdering.Events.Domain.CustomerInformations;

public class CustomerInformationCreatedEvent : BaseEvent
{
    public Guid CustomerId { get; set; }

    public override string GetEventType() => nameof(CustomerInformationCreatedEvent);
}
