using Application.Dtos.Ingredients;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class IngredientMappingProfile : Profile
{
    public IngredientMappingProfile()
    {
        // Map from IngredientCreateDto to Ingredient
        CreateMap<IngredientCreateDto, Ingredient>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

        // Map from Ingredient to IngredientReadDto
        CreateMap<Ingredient, IngredientReadDto>()
            .ForMember(
                dest => dest.Tags,
                opt =>
                    opt.MapFrom(src => src.IngredientTagRels.Select(rel => rel.Tag.Name).ToList())
            )
            .ForMember(
                dest => dest.Allergens,
                opt =>
                    opt.MapFrom(src =>
                        src.IngredientAllergenRels
                            .Select(rel => new IngredientAllergenDto
                            {
                                Id = rel.Allergen.Id,
                                Name = rel.Allergen.Name,
                                EuNumber = rel.Allergen.EuNumber,
                            })
                            .ToList()
                    )
            );

        // Map from IngredientUpdateDto to Ingredient (Only update non-null properties)
        CreateMap<IngredientUpdateDto, Ingredient>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
