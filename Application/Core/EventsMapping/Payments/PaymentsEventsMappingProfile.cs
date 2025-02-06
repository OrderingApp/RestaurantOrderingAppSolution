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

        CreateMap<(Payment payment, PaymentStatus previousPaymentStatus), PaymentStatusChangedEvent>()
           .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.payment.Id))
           .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.previousPaymentStatus))
           .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.payment.PaymentStatus));

    }
}
