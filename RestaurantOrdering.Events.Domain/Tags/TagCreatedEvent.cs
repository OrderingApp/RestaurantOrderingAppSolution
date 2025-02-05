namespace RestaurantOrdering.Events.Domain.Tags;

public class TagCreatedEvent : BaseEvent
{
    public Guid TagId { get; set; }

    public override string GetEventType() => nameof(TagCreatedEvent);
}
