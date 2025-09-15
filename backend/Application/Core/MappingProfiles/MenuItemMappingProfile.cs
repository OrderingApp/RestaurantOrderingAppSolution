using Application.Dtos.MenuItems;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuItemMappingProfile : Profile
{
    public MenuItemMappingProfile()
    {
        CreateMap<MenuItem, MenuItemReadDto>()
            .ForMember(d => d.Ingredients,
                o => o.MapFrom(s => s.MenuItemIngredientRels.Select(rel => rel.Ingredient)));

        CreateMap<Ingredient, MenuItemIngredientBasicDto>();

        CreateMap<Ingredient, MenuItemIngredientWithTagsDto>()
            .ForMember(d => d.TagIds,
                o => o.MapFrom(s => s.IngredientTagRels.Select(r => r.TagId)));

        CreateMap<MenuItem, MenuItemDetailedDto>()
            .ForMember(d => d.BaseIngredients,
                o => o.MapFrom(s => s.MenuItemIngredientRels.Select(rel => rel.Ingredient)));

        CreateMap<MenuItemCreateDto, MenuItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.MenuItemIngredientRels, opt => opt.Ignore());

        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
