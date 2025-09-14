using Application.Dtos.MenuItems;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuItemMappingProfile : Profile
{
    public MenuItemMappingProfile()
    {
        CreateMap<MenuItem, MenuItemReadDto>()
            .ForMember(
                dest => dest.Ingredients,
                opt =>
                    opt.MapFrom(src =>
                        src.MenuItemIngredientRels.Select(mii => mii.Ingredient).ToList()
                    )
            );

        CreateMap<MenuItemIngredientRel, MenuItemIngredientReadDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.IngredientId))
            .ForMember(d => d.Name, o => o.MapFrom(s => s.Ingredient.Name));

        CreateMap<MenuItem, MenuItemDetailedDto>()
            .ForMember(d => d.BaseIngredients, o => o.MapFrom(s => s.MenuItemIngredientRels));

        CreateMap<MenuItemCreateDto, MenuItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.MenuItemIngredientRels, opt => opt.Ignore());

        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
