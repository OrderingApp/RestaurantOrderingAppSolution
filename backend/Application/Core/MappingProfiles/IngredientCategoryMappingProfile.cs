using Application.Dtos.IngredientCategories;
using Application.Dtos.Ingredients;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class IngredientCategoryMappingProfile : Profile
{
    public IngredientCategoryMappingProfile()
    {
        CreateMap<IngredientCategoryCreateDto, IngredientCategory>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsUsed, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<IngredientCategory, IngredientCategoryReadDto>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<IngredientCategoryUpdateDto, IngredientCategory>();
    }
}
