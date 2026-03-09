namespace RestaurantOrdering.Events.Domain.Allergens;

public class AllergenUpdatedEvent : BaseEvent
{
    public Guid AllergenId { get; set; }

    public override string GetEventType() => nameof(AllergenUpdatedEvent);
}
