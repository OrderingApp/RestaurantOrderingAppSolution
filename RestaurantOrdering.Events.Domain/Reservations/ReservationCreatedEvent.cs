namespace RestaurantOrdering.Events.Domain.Reservations;

public class ReservationCreatedEvent : BaseEvent
{
    public Guid ReservationId { get; set; }

    public override string GetEventType() => nameof(ReservationCreatedEvent);
}
