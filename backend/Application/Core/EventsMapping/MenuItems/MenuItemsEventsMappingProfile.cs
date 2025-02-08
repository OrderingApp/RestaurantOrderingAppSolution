using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.MenuItems;

namespace Application.Core.EventsMapping.MenuItems;

public class MenuItemsEventsMappingProfile : Profile
{
    public MenuItemsEventsMappingProfile()
    {
        CreateMap<MenuItem, MenuItemCreatedEvent>()
            .ForMember(dest => dest.MenuItemId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MenuItem, MenuItemUpdatedEvent>()
            .ForMember(dest => dest.MenuItemId, opt => opt.MapFrom(src => src.Id));

        CreateMap<MenuItem, MenuItemDeletedEvent>()
            .ForMember(dest => dest.MenuItemId, opt => opt.MapFrom(src => src.Id));
    }
}
