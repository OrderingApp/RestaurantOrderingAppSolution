using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Orders;

namespace Application.Core.EventsMapping.Orders.CreatingOrder;

public class DeliveryOrderCreatedMapping : Profile
{
    public DeliveryOrderCreatedMapping()
    {
        CreateMap<Order, DeliveryOrderCreatedEvent>();
    }
}
