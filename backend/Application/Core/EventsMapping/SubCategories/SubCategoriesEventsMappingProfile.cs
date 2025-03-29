using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.MenuCategories;
using RestaurantOrdering.Events.Domain.SubCategories;

namespace Application.Core.EventsMapping.SubCategories;

public class SubCategoriesEventsMappingProfile : Profile
{
    public SubCategoriesEventsMappingProfile()
    {
        CreateMap<SubCategory, SubCategoryCreatedEvent>()
            .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.Id));

        CreateMap<SubCategory, SubCategoryUpdatedEvent>()
            .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.Id));

        CreateMap<SubCategory, SubCategoryDeletedEvent>()
            .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.Id));
    }
}
