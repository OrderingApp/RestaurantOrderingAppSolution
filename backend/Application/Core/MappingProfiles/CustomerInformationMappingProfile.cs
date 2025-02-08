using Application.Dtos.CustomerInformations;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class CustomerInformationMappingProfile : Profile
{
    public CustomerInformationMappingProfile()
    {
        // Mapping from Domain to Read DTO
        CreateMap<CustomerInformation, CustomerInformationReadDto>();

        // Mapping from Create DTO to Domain
        CreateMap<CustomerInformationCreateDto, CustomerInformation>();

        // Mapping from Update DTO to Domain
        CreateMap<CustomerInformationUpdateDto, CustomerInformation>();
    }
}
