namespace RestaurantOrdering.Events.Domain.Allergens;

public class AllergenCreatedEvent : BaseEvent
{
    public Guid AllergenId { get; set; }

    public override string GetEventType() => nameof(AllergenCreatedEvent);
}
