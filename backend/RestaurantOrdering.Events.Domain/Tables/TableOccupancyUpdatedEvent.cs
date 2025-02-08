namespace RestaurantOrdering.Events.Domain.Tables;

public class TableOccupancyUpdatedEvent : BaseEvent
{
    public Guid TableId { get; set; }
    public bool NewOccupancy { get; set; }

    public override string GetEventType() => nameof(TableOccupancyUpdatedEvent);
}
