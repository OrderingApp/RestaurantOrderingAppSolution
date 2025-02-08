namespace RestaurantOrdering.Events.Domain.MenuItems;

public class MenuItemDeletedEvent : BaseEvent
{
    public Guid MenuItemId { get; set; }

    public override string GetEventType() => nameof(MenuItemDeletedEvent);
}
