namespace RestaurantOrdering.Events.Domain.Tags;

public class TagDeletedEvent : BaseEvent
{
    public Guid TagId { get; set; }

    public override string GetEventType() => nameof(TagDeletedEvent);
}
