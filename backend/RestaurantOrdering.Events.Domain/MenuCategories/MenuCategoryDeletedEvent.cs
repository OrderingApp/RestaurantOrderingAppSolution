namespace RestaurantOrdering.Events.Domain.MenuCategories;

public class MenuCategoryDeletedEvent : BaseEvent
{
    public Guid CategoryId { get; set; }

    public override string GetEventType() => nameof(MenuCategoryDeletedEvent);
}
