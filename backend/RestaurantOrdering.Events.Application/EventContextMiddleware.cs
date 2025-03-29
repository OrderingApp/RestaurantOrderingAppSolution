using System.Text.Json;
using RestaurantOrdering.Events.Application.Orders.OrderModifications;
using RestaurantOrdering.Events.Domain;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;
using RestaurantOrdering.Events.Infrastructure.Database;

namespace RestaurantOrdering.Events.Application;

public class EventContextMiddleware(OrderClosedEventHandler orderClosedEventHandler)
    : IEventContextMiddleware
{
    public async void HandleEventContextAdded(EventContext eventContext)
    {
        Console.WriteLine($"New EventContext added: {eventContext.Id}");

        switch (eventContext.EventType)
        {
            case nameof(OrderClosedEvent):
            {
                Console.WriteLine($"Handling: {nameof(OrderClosedEvent)}");
                var eventToHandle = JsonSerializer.Deserialize<OrderClosedEvent>(
                    eventContext.Payload
                );
                await orderClosedEventHandler.HandleEventAsync(eventToHandle);
                break;
            }
        }
    }
}
