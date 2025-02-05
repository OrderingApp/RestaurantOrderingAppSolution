using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Orders.CreatingOrder;
using RestaurantOrdering.Events.Domain.Orders.DiscountsOrder;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.StatusUpdates;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder.TypeUpdates;
using RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;
using RestaurantOrdering.Events.Domain.Orders.PaymentsOrder.PaymentStatusUpdates;

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

        CreateMap<Order, OrderPaymentStatusPaidEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderSplitBillEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderTableChangedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderDiscountAppliedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderStatusCancelledEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Order, OrderStatusFinishedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Order, OrderStatusOngoingEvent>()
           .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Order, OrderTypeTakeawayEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Order, OrderTypeDineInEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
        CreateMap<Order, OrderTypeDeliveryEvent>()
           .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id));
    }
}
