namespace RestaurantOrdering.Events.Domain.MenuCategories;

public class MenuCategoryCreatedEvent : BaseEvent
{
    public Guid CategoryId { get; set; }

    public override string GetEventType() => nameof(MenuCategoryCreatedEvent);
}
