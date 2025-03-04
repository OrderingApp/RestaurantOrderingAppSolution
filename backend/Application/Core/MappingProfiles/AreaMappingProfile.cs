using Application.Dtos.Areas;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class AreaMappingProfile : Profile
{
    public AreaMappingProfile()
    {
        CreateMap<AreaCreateDto, Area>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.IsUsed, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

        CreateMap<Area, AreaReadDto>();

        CreateMap<AreaUpdateDto, Area>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
