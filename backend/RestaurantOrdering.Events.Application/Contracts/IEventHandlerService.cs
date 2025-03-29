using RestaurantOrdering.Events.Domain;

namespace RestaurantOrdering.Events.Application.Contracts;

public interface IEventHandlerService
{
    Task HandleEventAsync<TEvent>(TEvent eventToSend)
        where TEvent : BaseEvent;
}
