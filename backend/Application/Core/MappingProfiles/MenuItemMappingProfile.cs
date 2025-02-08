using Application.Dtos.MenuItems;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuItemMappingProfile : Profile
{
    public MenuItemMappingProfile()
    {
        CreateMap<MenuItem, MenuItemReadDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.MenuItemTags.Select(mt => mt.Tag)
            .ToList()));

        CreateMap<MenuItem, MenuItemDetailedDto>()
            .ForMember(dest => dest.MenuCategoryName, opt => opt.MapFrom(src => src.MenuCategory!.Name));

        CreateMap<MenuItemCreateDto, MenuItem>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.MenuItemTags, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<MenuItemUpdateDto, MenuItem>()
            .ForMember(dest => dest.MenuItemTags, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
