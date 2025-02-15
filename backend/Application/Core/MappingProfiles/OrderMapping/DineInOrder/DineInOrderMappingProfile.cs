using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDineIn;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles.OrderMapping.DineInOrder;

public class DineInOrderMappingProfile : Profile
{
    public DineInOrderMappingProfile()
    {
        CreateMap<Order, OrderReadDto>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.CustomerInformation, opt => opt.MapFrom(src => src.CustomerInformation));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<DineInOrderCreateDto, Order>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => OrderType.DineIn))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.TableId));
    }
}
