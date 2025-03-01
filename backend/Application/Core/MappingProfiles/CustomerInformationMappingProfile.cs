using Application.Dtos.CustomerInformations;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class CustomerInformationMappingProfile : Profile
{
    public CustomerInformationMappingProfile()
    {
        CreateMap<CustomerInformationCreateDto, CustomerInformation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

        CreateMap<CustomerInformation, CustomerInformationReadDto>();

        CreateMap<CustomerInformationUpdateDto, CustomerInformation>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
