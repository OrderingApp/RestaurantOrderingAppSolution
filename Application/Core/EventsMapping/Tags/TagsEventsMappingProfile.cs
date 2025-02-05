using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Tags;

namespace Application.Core.EventsMapping.Tags;

public class TagsEventsMappingProfile : Profile
{
    public TagsEventsMappingProfile()
    {
        CreateMap<Tag, TagCreatedEvent>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Tag, TagUpdatedEvent>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Tag, TagDeletedEvent>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id));
    }
}
