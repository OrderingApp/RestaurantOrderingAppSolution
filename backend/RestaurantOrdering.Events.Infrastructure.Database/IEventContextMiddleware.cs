using RestaurantOrdering.Events.Domain;

namespace RestaurantOrdering.Events.Infrastructure.Database;

public interface IEventContextMiddleware
{
    void HandleEventContextAdded(EventContext eventContext);
}

