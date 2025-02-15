using Application.Dtos.OrderItemIngredients;
using Application.Dtos.OrderItems;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        //CreateMap<OrderItem, OrderItemReadDto>()
        //    .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
        //    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.OrderItemIngredients));

        //CreateMap<OrderItem, OrderItemSummaryDto>()
        //    .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
        //    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.OrderItemIngredients.Select(ingredient => new OrderItemIngredientReadDto
        //    {
        //        IngredientId = ingredient.IngredientId,
        //        IngredientName = ingredient.Ingredient.Name,
        //        Quantity = ingredient.Quantity,
        //        Price = ingredient.Ingredient.Price
        //    }).ToList()));

        //CreateMap<OrderItem, OrderItemsListDto>()
        //    .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
        //    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.OrderItemIngredients));

        //CreateMap<OrderItemCreateDto, OrderItem>()
        //    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
        //    .ForMember(dest => dest.OrderId, opt => opt.Ignore())
        //    .ForMember(dest => dest.Price, opt => opt.Ignore())
        //    .ForMember(dest => dest.MenuItemId, opt => opt.MapFrom(src => src.MenuItemId))
        //    .ForMember(dest => dest.OrderItemIngredients, opt => opt.MapFrom(src => src.Ingredients));

        //CreateMap<OrderItemUpdateDto, OrderItem>();

        //CreateMap<MenuItemIngredientRel, OrderItemIngredientReadDto>()
        //    .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient.Name))
        //    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Ingredient.Price));

        //CreateMap<OrderItemIngredientAddDto, MenuItemIngredientRel>()
        //    .ForMember(dest => dest.OrderItemId, opt => opt.Ignore())
        //    .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.IngredientId))
        //    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}
