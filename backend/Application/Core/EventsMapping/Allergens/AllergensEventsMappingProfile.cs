using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Allergens;

namespace Application.Core.EventsMapping.Allergens;

public class AllergensEventsMappingProfile : Profile
{
    public AllergensEventsMappingProfile()
    {
        CreateMap<Allergen, AllergenCreatedEvent>()
            .ForMember(dest => dest.AllergenId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Allergen, AllergenUpdatedEvent>()
            .ForMember(dest => dest.AllergenId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Allergen, AllergenDeletedEvent>()
            .ForMember(dest => dest.AllergenId, opt => opt.MapFrom(src => src.Id));
    }
}
