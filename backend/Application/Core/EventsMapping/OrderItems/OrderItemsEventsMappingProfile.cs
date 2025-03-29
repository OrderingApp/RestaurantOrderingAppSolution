using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.OrderItems;

namespace Application.Core.EventsMapping.OrderItems;

public class OrderItemsEventsMappingProfile : Profile
{
    public OrderItemsEventsMappingProfile()
    {
        CreateMap<(Guid orderId, List<OrderItem> orderItems), OrderItemAddedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.orderId))
            .ForMember(
                dest => dest.OrderItemIds,
                opt => opt.MapFrom(src => src.orderItems.Select(oi => oi.Id).ToList())
            );

        CreateMap<List<OrderItem>, OrderItemAddedEvent>()
            .ForMember(
                dest => dest.OrderItemIds,
                opt => opt.MapFrom(src => src.Select(oi => oi.Id).ToList())
            );

        CreateMap<OrderItem, OrderItemUpdatedEvent>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(
                dest => dest.SpecialInstructions,
                opt => opt.MapFrom(src => src.SpecialInstructions)
            )
            .ForMember(
                dest => dest.ExtraIngredientIds,
                opt => opt.MapFrom(src => src.ExtraIngredients.Select(e => e.Id).ToList())
            )
            .ForMember(
                dest => dest.RemovedIngredientIds,
                opt => opt.MapFrom(src => src.RemovedIngredients.Select(r => r.Id).ToList())
            );

        CreateMap<OrderItem, OrderItemDeletedEvent>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id));

        CreateMap<
            (OrderItem orderItem, OrderItemStatus previousStatus),
            OrderItemStatusUpdatedEvent
        >()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.orderItem.Id))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.orderItem.OrderId))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.previousStatus))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.orderItem.Status));
    }
}
