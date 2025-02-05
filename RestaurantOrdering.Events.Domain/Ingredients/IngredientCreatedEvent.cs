namespace RestaurantOrdering.Events.Domain.Ingredients;

public class IngredientCreatedEvent : BaseEvent
{
    public Guid IngredientId { get; set; }

    public override string GetEventType() => nameof(IngredientCreatedEvent);
}
