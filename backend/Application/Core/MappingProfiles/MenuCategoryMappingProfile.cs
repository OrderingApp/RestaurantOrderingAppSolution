using Application.Dtos.MenuCategories;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuCategoryMappingProfile : Profile
{
    public MenuCategoryMappingProfile()
    {
        CreateMap<MenuCategory, MenuCategoryReadDto>();

        CreateMap<MenuCategory, MenuCategoryHierarchyReadDto>()
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
            .ForMember(dest => dest.MenuItems, opt => opt.MapFrom(src => src.MenuItems))
            .ForMember(dest => dest.TotalItems,
                opt => opt.MapFrom(src => src.MenuItems.Count(mi => mi.IsUsed && !mi.IsDeleted)));

        CreateMap<MenuCategoryCreateDto, MenuCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<MenuCategoryUpdateDto, MenuCategory>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
