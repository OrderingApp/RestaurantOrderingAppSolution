using Domain;

namespace RestaurantOrdering.Events.Domain.Tables;

public class TableStatusUpdatedEvent : BaseEvent
{
    public Guid TableId { get; set; }
    public TableStatus From { get; set; }
    public TableStatus To { get; set; }

    public override string GetEventType() => nameof(TableStatusUpdatedEvent);
}
