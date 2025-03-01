using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class SplitOrderMappingProfile : Profile
{
    public SplitOrderMappingProfile()
    {
        // Mapping for cloning Order with a new ID and excluding OrderItems (handled separately)
        CreateMap<Order, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerInformation, opt => opt.Ignore());

        // Mapping for cloning OrderItem with a new ID and preserving relevant properties
        CreateMap<OrderItem, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.MenuItem, opt => opt.MapFrom(src => src.MenuItem))
            .ForMember(dest => dest.ExtraIngredients, opt => opt.MapFrom(src => src.ExtraIngredients))
            .ForMember(dest => dest.RemovedIngredients, opt => opt.MapFrom(src => src.RemovedIngredients));

        // Mapping for cloning OrderItemIngredient with a new ID
        CreateMap<OrderItemIngredient, OrderItemIngredient>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
    }
}
