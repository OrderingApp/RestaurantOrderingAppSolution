using Application.Dtos.Orders;
using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;

namespace Application.Core.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderUpdateTypeDto, CustomerInformation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.AdditionalInstructions, opt => opt.MapFrom(src => src.AdditionalInstructions));

        CreateMap<OrderCloseDto, OrderClosedEvent>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments));

        CreateMap<Order, NonDineInOrderSummaryDto>()
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.OrderType, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.CustomerInformation != null ? src.CustomerInformation.PhoneNumber : null))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.CustomerInformation != null ? src.CustomerInformation.Address : null));
    }
}