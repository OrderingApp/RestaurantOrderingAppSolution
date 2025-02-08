namespace RestaurantOrdering.Events.Domain.CustomerInformations;

public class CustomerInformationUpdatedEvent : BaseEvent
{
    public Guid CustomerId { get; set; }

    public override string GetEventType() => nameof(CustomerInformationUpdatedEvent);
}
