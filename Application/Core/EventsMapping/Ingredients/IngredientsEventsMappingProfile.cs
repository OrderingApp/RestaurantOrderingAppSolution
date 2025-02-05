using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Ingredients;

namespace Application.Core.EventsMapping.Ingredients;

public class IngredientsEventsMappingProfile : Profile
{
    public IngredientsEventsMappingProfile()
    {
        CreateMap<Ingredient, IngredientCreatedEvent>()
            .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Ingredient, IngredientUpdatedEvent>()
            .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Ingredient, IngredientDeletedEvent>()
            .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.Id));
    }
}
