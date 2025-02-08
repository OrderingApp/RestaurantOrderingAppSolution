using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.CustomerInformations;

namespace Application.Core.EventsMapping.CustomerInformations;

public class CustomerInformationsEventsMappingProfile : Profile
{
    public CustomerInformationsEventsMappingProfile()
    {
        CreateMap<CustomerInformation, CustomerInformationCreatedEvent>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));

        CreateMap<CustomerInformation, CustomerInformationUpdatedEvent>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));

        CreateMap<CustomerInformation, CustomerInformationDeletedEvent>()
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id));
    }
}
