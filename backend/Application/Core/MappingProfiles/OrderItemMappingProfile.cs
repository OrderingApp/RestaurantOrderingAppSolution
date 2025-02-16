using Application.Dtos.OrderItems;
using Application.Dtos.OrderItemIngredients;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItem, OrderItemReadDto>()
            .ForMember(dest => dest.MenuItem, opt => opt.MapFrom(src => src.MenuItem))
            .ForMember(dest => dest.ExtraIngredients, opt => opt.MapFrom(src => src.ExtraIngredients))
            .ForMember(dest => dest.RemovedIngredients, opt => opt.MapFrom(src => src.RemovedIngredients));

        CreateMap<OrderItemCreateDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => OrderItemStatus.Pending))
            .ForMember(dest => dest.ExtraIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedIngredients, opt => opt.Ignore());

        CreateMap<OrderItemUpdateDto, OrderItem>()
            .ForMember(dest => dest.ExtraIngredients, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedIngredients, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<OrderItemIngredient, OrderItemIngredientReadDto>()
            .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        CreateMap<OrderItemIngredientAddDto, OrderItemIngredient>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

        CreateMap<OrderItem, OrderItemsListDto>()
            .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name));
    }
}
