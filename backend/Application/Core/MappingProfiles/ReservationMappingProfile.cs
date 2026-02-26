using Application.Dtos.Reservations;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class ReservationMappingProfile : Profile
{
    public ReservationMappingProfile()
    {
        // Mapping from Reservation to ReservationReadDto
        CreateMap<Reservation, ReservationReadDto>();

        // Mapping from ReservationCreateDto to Reservation
        CreateMap<ReservationCreateDto, Reservation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()));

        // Mapping from ReservationUpdateDto to Reservation
        CreateMap<ReservationUpdateDto, Reservation>()
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.TableId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Reservation, ReservationSummaryDto>();
    }
}
