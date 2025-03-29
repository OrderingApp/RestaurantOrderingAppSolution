using Application.Dtos.Orders.OrderTakeAway;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles.OrderMapping.TakeawayOrder;

public class TakeawayOrderMappingProfile : Profile
{
    public TakeawayOrderMappingProfile()
    {
        CreateMap<TakeawayOrderCreateDto, Order>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => OrderType.Takeaway))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(
                dest => dest.CustomerInformation,
                opt => opt.MapFrom(src => src.CustomerInformation)
            );
    }
}
