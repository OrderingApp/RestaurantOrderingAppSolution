using Application.Dtos.Orders.OrderDelivery;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles.OrderMapping.DeliveryOrder;

public class DeliveryOrderMappingProfile : Profile
{
    public DeliveryOrderMappingProfile()
    {
        CreateMap<DeliveryOrderCreateDto, Order>()
            .ForMember(d => d.Type, o => o.MapFrom(_ => OrderType.Delivery))
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.TotalAmount, o => o.Ignore())
            .ForMember(d => d.Status, o => o.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.Discount, o => o.MapFrom(s => s.Discount ?? 0m))
            .ForMember(d => d.DeliveryPrice, o => o.MapFrom(s => s.DeliveryPrice ?? 0m))
            .ForMember(d => d.CustomerInformation, o => o.MapFrom(s => s.CustomerInformation));
    }
}
