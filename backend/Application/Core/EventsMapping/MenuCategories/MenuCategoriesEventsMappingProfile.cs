using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.MenuCategories;

namespace Application.Core.EventsMapping.MenuCategories;

public class MenuCategoriesEventsMappingProfile : Profile
{
    public MenuCategoriesEventsMappingProfile()
    {
        CreateMap<MenuCategory, MenuCategoryCreatedEvent>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MenuCategory, MenuCategoryUpdatedEvent>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MenuCategory, MenuCategoryDeletedEvent>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id));
    }
}
