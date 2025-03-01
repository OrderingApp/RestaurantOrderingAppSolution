using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Orders.CreatingOrder;
using RestaurantOrdering.Events.Domain.Orders.DiscountsOrder;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;
using RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;

namespace Application.Core.EventsMapping.Orders;

public class OrdersEventsMappingProfile : Profile
{
    public OrdersEventsMappingProfile()
    {
        CreateMap<Order, DeliveryOrderCreatedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Order, TakeawayOrderCreatedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Order, DineInOrderCreatedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderSplitBillEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderTableChangedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderDiscountAppliedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderClosedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));

        CreateMap<(Order order, OrderStatus previousOrderStatus), OrderStatusChangedEvent>()
           .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.order.Id))
           .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.previousOrderStatus))
           .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.order.Status));

        CreateMap<(Order order, OrderType previousOrderType), OrderTypeChangeEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.order.Id))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.previousOrderType))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.order.Type));
    }
}
