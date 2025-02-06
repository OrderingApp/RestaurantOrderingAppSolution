using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderCreate;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderReadDto>()
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType.ToString()))
            .ForMember(dest => dest.CustomerInformation, opt => opt.MapFrom(src => src.CustomerInformation));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.OrderType.ToString()));

        CreateMap<DineInOrderCreateDto, Order>()
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(_ => OrderType.DineIn))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<TakeawayOrderCreateDto, Order>()
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(_ => OrderType.Takeaway))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.CustomerInformation, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<TakeawayOrderCreateDto, CustomerInformation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.AdditionalInstructions, opt => opt.MapFrom(src => src.AdditionalInstructions))
            .ForMember(dest => dest.Address, opt => opt.Ignore());

        CreateMap<DeliveryOrderCreateDto, Order>()
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(_ => OrderType.Delivery))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(_ => OrderStatus.Ongoing))
            .ForMember(dest => dest.CustomerInformation, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

        CreateMap<DeliveryOrderCreateDto, CustomerInformation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.AdditionalInstructions, opt => opt.MapFrom(src => src.AdditionalInstructions));

        CreateMap<OrderUpdateTypeDto, CustomerInformation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.AdditionalInstructions, opt => opt.MapFrom(src => src.AdditionalInstructions));
    }
}