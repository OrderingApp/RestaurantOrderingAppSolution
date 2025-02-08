using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.OrderItems;

namespace Application.Core.EventsMapping.OrderItems;

public class OrderItemsEventsMappingProfile : Profile
{
    public OrderItemsEventsMappingProfile()
    {
        CreateMap<OrderItem, OrderItemAddedEvent>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id));

        CreateMap<OrderItem, OrderItemUpdatedEvent>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id));

        CreateMap<OrderItem, OrderItemDeletedEvent>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id));

        CreateMap<(OrderItem orderItem, decimal discountPercentage), OrderItemDiscountAppliedEvent>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.orderItem.Id))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.discountPercentage));

    }
}
