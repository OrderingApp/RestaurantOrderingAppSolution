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
                src.MenuItemIngredientRels
                    .Select(rel => new MenuItemIngredientReadDto
                    {
                        Id = rel.Ingredient.Id,
                        Name = rel.Ingredient.Name,
                    }).ToList()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));

        CreateMap<MenuItemCreateDto, MenuItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.MenuItemIngredientRels, opt => opt.Ignore());

        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}