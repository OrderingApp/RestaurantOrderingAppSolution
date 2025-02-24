using Application.Dtos.Orders.OrderDelivery;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles.OrderMapping.DeliveryOrder;

public class DeliveryOrderMappingProfile : Profile
{
    public DeliveryOrderMappingProfile()
    {
        CreateMap<DeliveryOrderCreateDto, Order>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => OrderType.Delivery))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerInformation, opt => opt.MapFrom(src => src.CustomerInformation));
    }
}