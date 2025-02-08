namespace RestaurantOrdering.Events.Domain.Tables;

public class TableCreatedEvent : BaseEvent
{
    public Guid TableId { get; set; }

    public override string GetEventType() => nameof(TableCreatedEvent);
}
