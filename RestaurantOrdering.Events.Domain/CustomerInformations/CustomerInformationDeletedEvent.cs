namespace RestaurantOrdering.Events.Domain.CustomerInformations;

public class CustomerInformationDeletedEvent : BaseEvent
{
    public Guid CustomerId { get; set; }

    public override string GetEventType() => nameof(CustomerInformationDeletedEvent);
}
