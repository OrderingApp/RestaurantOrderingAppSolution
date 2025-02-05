namespace RestaurantOrdering.Events.Domain.MenuItems;

public class MenuItemUpdatedEvent : BaseEvent
{
    public Guid MenuItemId { get; set; }

    public override string GetEventType() => nameof(MenuItemUpdatedEvent);
}
