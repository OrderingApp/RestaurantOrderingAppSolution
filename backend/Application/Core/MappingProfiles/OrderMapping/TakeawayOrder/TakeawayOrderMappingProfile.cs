using Application.Dtos.Orders.OrderTakeAway;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles.OrderMapping.TakeawayOrder;

public class TakeawayOrderMappingProfile : Profile
{
    public TakeawayOrderMappingProfile()
    {
        CreateMap<TakeawayOrderCreateDto, Order>()
            .ForMember(d => d.Type, o => o.MapFrom(_ => OrderType.Takeaway))
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.TotalAmount, o => o.Ignore())
            .ForMember(d => d.Status, o => o.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(d => d.OrderItems, o => o.Ignore())
            .ForMember(d => d.Discount, o => o.MapFrom(s => s.Discount ?? 0m))
            .ForMember(d => d.CustomerInformation, o => o.MapFrom(s => s.CustomerInformation));
    }
}
