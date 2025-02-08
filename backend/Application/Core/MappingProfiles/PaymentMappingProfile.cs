using Application.Dtos.Payments;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class PaymentMappingProfile : Profile
{
    public PaymentMappingProfile()
    {
        CreateMap<Payment, PaymentReadDto>();

        CreateMap<PaymentCreateDto, Payment>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));
    }
}
