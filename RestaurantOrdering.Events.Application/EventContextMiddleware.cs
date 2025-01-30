using RestaurantOrdering.Events.Domain;
using RestaurantOrdering.Events.Domain.Orders;

namespace RestaurantOrdering.Events.Application;

public class EventContextMiddleware
{
    public void HandleEventContextAdded(EventContext entity)
    {
        Console.WriteLine($"Middleware triggered for new EventContext: {entity}");

        //switch (entity.EventType)
        //{
        //    case nameof(DeliveryOrderCreatedEvent):
        //        {

        //        }
        //}
    }
}