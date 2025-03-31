using System.Text.Json;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain;
using RestaurantOrdering.Events.Infrastructure.Database;

namespace RestaurantOrdering.Events.Application;

public class EventHandlerService(EventsDatabaseContext eventsDatabaseContext) : IEventHandlerService
{
    public async Task HandleEventAsync<TEvent>(TEvent eventToSend)
        where TEvent : BaseEvent
    {
        var eventContext = new EventContext
        {
            CorrelationId = Guid.NewGuid(),
            DateTime = DateTime.Now,
            EventType = eventToSend.GetEventType(),
            PayloadJson = JsonSerializer.Serialize(eventToSend),
        };

        await eventsDatabaseContext.EventContexts.AddAsync(eventContext);
        await eventsDatabaseContext.SaveChangesAsync();
    }
}
