using Application.Dtos.MenuItems;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuItemMappingProfile : Profile
{
    public MenuItemMappingProfile()
    {
        CreateMap<MenuItem, MenuItemReadDto>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src =>
                src.MenuItemIngredientRels.Select(mii => mii.Ingredient).ToList()
            ));


        CreateMap<Ingredient, MenuItemIngredientReadDto>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MenuItemCreateDto, MenuItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.MenuItemIngredientRels, opt => opt.Ignore());

        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}