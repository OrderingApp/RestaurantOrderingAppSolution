namespace RestaurantOrdering.Events.Domain.Allergens;

public class AllergenDeletedEvent : BaseEvent
{
    public Guid AllergenId { get; set; }

    public override string GetEventType() => nameof(AllergenDeletedEvent);
}
