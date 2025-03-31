using Application.Dtos.MenuItems;
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

        CreateMap<Tag, IngredientTagReadDto>();

        // Map from TagUpdateDto to Tag
        CreateMap<TagUpdateDto, Tag>();
    }
}
