using Application.Dtos.Orders.OrderDelivery;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles.OrderMapping.DeliveryOrder;

public class DeliveryOrderMappingProfile : Profile
{
    public DeliveryOrderMappingProfile()
    {
        CreateMap<Order, DeliveryOrderSummaryReadDto>()
            .ForMember(dest => dest.DeliveryOrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.OrderTotal, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.CustomerInformation, opt => opt.MapFrom(src => src.CustomerInformation));

        CreateMap<DeliveryOrderCreateDto, Order>()
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(_ => OrderType.Delivery))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.CustomerInformation, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<DeliveryOrderCreateDto, CustomerInformation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.AdditionalInstructions, opt => opt.MapFrom(src => src.AdditionalInstructions))
            .ForMember(dest => dest.OrderCompletionType, opt => opt.MapFrom(src => src.OrderCompletionType))
            .ForMember(dest => dest.PreferedPaymentMethod, opt => opt.MapFrom(src => src.PreferedPaymentMethod))
            .ForMember(dest => dest.ExpectedOrderCompletion, opt => opt.MapFrom(src =>
                src.OrderCompletionType == OrderCompletionType.Scheduled ? src.ExpectedOrderCompletion : null));
    }
}
