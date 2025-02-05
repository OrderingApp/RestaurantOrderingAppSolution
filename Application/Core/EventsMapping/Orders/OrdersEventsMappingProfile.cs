using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Orders.CreatingOrder;

namespace Application.Core.EventsMapping.Orders;

public class OrdersEventsMappingProfile : Profile
{
    public OrdersEventsMappingProfile()
    {
        CreateMap<Order, DeliveryOrderCreatedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, TakeawayOrderCreatedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
    }
}
