using Application.Dtos.Allergens;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class AllergenMappingProfile : Profile
{
    public AllergenMappingProfile()
    {
        CreateMap<AllergenCreateDto, Allergen>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsUsed, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<Allergen, AllergenReadDto>();

        CreateMap<AllergenUpdateDto, Allergen>();
    }
}
