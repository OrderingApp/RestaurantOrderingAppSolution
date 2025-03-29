using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Payments;

namespace Application.Core.EventsMapping.Payments;

public class PaymentsEventsMappingProfile : Profile
{
    public PaymentsEventsMappingProfile()
    {
        CreateMap<Payment, PaymentCreatedEvent>()
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id));
    }
}
