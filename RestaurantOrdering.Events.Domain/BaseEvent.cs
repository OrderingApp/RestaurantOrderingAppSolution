namespace RestaurantOrdering.Events.Domain;

public abstract class BaseEvent
{
    public abstract string GetEventType();
}
