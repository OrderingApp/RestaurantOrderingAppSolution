namespace RestaurantOrdering.Events.Domain.MenuCategories;

public class MenuCategoryUpdatedEvent : BaseEvent
{
    public Guid CategoryId { get; set; }

    public override string GetEventType() => nameof(MenuCategoryUpdatedEvent);
}
