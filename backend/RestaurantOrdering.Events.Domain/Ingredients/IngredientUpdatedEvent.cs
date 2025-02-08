namespace RestaurantOrdering.Events.Domain.Ingredients;

public class IngredientUpdatedEvent : BaseEvent
{
    public Guid IngredientId { get; set; }

    public override string GetEventType() => nameof(IngredientUpdatedEvent);
}
