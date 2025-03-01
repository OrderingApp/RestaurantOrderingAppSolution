using Application.Dtos.MenuCategories;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class MenuCategoryMappingProfile : Profile
{
    public MenuCategoryMappingProfile()
    {
        CreateMap<MenuCategory, MenuCategoryReadDto>();

        CreateMap<MenuCategoryCreateDto, MenuCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<MenuCategoryUpdateDto, MenuCategory>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
