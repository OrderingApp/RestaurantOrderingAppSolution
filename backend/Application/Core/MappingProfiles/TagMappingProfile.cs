using Application.Dtos.MenuItemTags;
using Application.Dtos.Tags;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        // Map from TagCreateDto to Tag
        CreateMap<TagCreateDto, Tag>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsUsed, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        // Map from Tag to TagReadDto
        CreateMap<Tag, TagReadDto>();

        // Map from TagUpdateDto to Tag
        CreateMap<TagUpdateDto, Tag>();

        CreateMap<Tag, MenuItemTagReadDto>()
            .ForMember(dest => dest.TagId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Name));

    }
}
