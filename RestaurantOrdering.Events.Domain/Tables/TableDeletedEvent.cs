namespace RestaurantOrdering.Events.Domain.Tables;

public class TableDeletedEvent : BaseEvent
{
    public Guid TableId { get; set; }

    public override string GetEventType() => nameof(TableDeletedEvent);
}
