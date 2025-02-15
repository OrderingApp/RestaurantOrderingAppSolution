using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class SplitOrderMappingProfile : Profile
{
    public SplitOrderMappingProfile()
    {
        //CreateMap<OrderItem, OrderItem>()
        //    .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
        //    .ForMember(dest => dest.OrderItemIngredients, opt => opt.MapFrom(src =>
        //        src.OrderItemIngredients != null
        //        ? src.OrderItemIngredients.Select(ingredient => new MenuItemIngredientRel
        //        {
        //            IngredientId = ingredient.IngredientId,
        //            Quantity = ingredient.Quantity
        //        }).ToList()
        //        : new List<MenuItemIngredientRel>()));

        CreateMap<Order, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
    }
}
