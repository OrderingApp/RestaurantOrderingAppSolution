using Application.Dtos.SubCategories;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class SubCategoryMappingProfile : Profile
{
    public SubCategoryMappingProfile()
    {
        CreateMap<SubCategory, SubCategoryReadDto>()
            .ForMember(dest => dest.TotalItemsInSubCategory,
                opt => opt.MapFrom(src => src.MenuItems.Count(mi => mi.IsUsed && !mi.IsDeleted)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.MenuItems
                .SelectMany(mi => mi.MenuItemIngredientRels)
                .Select(mii => mii.Ingredient)
                .SelectMany(i => i.IngredientTagRels)
                .Select(it => it.Tag)
                .Distinct()
                .ToList()
            ));

        CreateMap<SubCategoryCreateDto, SubCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.MenuCategoryId, opt => opt.MapFrom(src => src.MenuCategoryId));

        CreateMap<SubCategory, SubCategoryUpdateDto>().ReverseMap();
    }
}
