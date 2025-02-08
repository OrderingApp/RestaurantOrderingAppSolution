namespace RestaurantOrdering.Events.Domain.Tables;

public class TableUpdatedEvent : BaseEvent
{
    public Guid TableId { get; set; }

    public override string GetEventType() => nameof(TableUpdatedEvent);
}
