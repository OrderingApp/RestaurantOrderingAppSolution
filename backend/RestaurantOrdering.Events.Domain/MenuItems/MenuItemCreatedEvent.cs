namespace RestaurantOrdering.Events.Domain.MenuItems;

public class MenuItemCreatedEvent : BaseEvent
{
    public Guid MenuItemId { get; set; }

    public override string GetEventType() => nameof(MenuItemCreatedEvent);
}
