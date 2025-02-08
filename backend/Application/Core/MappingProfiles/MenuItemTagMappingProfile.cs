using Application.Dtos.MenuItemTags;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuItemTagMappingProfile : Profile
{
    public MenuItemTagMappingProfile()
    {
        // Map from MenuItemTag to MenuItemTagReadDto
        CreateMap<MenuItemTag, MenuItemTagReadDto>()
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tag.Name));

        // Map from MenuItemTagCreateDto to MenuItemTag
        CreateMap<MenuItemTagCreateDto, MenuItemTag>();
    }
}
