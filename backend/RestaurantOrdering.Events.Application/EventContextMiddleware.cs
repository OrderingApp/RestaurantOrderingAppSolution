using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;
using RestaurantOrdering.Events.Domain;
using RestaurantOrdering.Events.Infrastructure.Database;

namespace RestaurantOrdering.Events.Application;

public class EventContextMiddleware : IEventContextMiddleware
{
    public void HandleEventContextAdded(EventContext eventContext)
    {
        Console.WriteLine($"New EventContext added: {eventContext.Id}");


        switch (eventContext.EventType)
        {
            case nameof(OrderClosedEvent):
                {
                    Console.WriteLine($"Handling: {nameof(OrderClosedEvent)}");
                    // some method from Application
                    break;
                }
        }
    }
}
