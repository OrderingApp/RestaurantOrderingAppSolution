namespace RestaurantOrdering.Events.Domain.Reservations;

public class ReservationUpdatedEvent : BaseEvent
{
    public Guid ReservationId { get; set; }

    public override string GetEventType() => nameof(ReservationUpdatedEvent);
}
