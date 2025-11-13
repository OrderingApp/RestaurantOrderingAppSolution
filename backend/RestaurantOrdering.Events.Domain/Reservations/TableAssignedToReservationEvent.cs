namespace RestaurantOrdering.Events.Domain.Reservations;

// change name
public class TableAssignedToReservationEvent : BaseEvent
{
    public Guid ReservationId { get; set; }
    public Guid TableId { get; set; }

    public override string GetEventType() => nameof(TableAssignedToReservationEvent);
}
