namespace RestaurantOrdering.Events.Domain.Reservations;

public class ReservationDeletedEvent : BaseEvent
{
    public Guid ReservationId { get; set; }

    public override string GetEventType() => nameof(ReservationDeletedEvent);
}