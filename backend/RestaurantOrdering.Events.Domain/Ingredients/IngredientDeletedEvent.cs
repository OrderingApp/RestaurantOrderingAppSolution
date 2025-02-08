namespace RestaurantOrdering.Events.Domain.Ingredients;

public class IngredientDeletedEvent : BaseEvent
{
    public Guid IngredientId { get; set; }

    public override string GetEventType() => nameof(IngredientDeletedEvent);
}
