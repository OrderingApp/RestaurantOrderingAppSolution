using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Reservations;

namespace Application.Core.EventsMapping.Reservations;

public class ReservationsEventsMappingProfile : Profile
{
    public ReservationsEventsMappingProfile()
    {
        CreateMap<Reservation, ReservationCreatedEvent>()
    .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Reservation, ReservationUpdatedEvent>()
            .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Reservation, ReservationDeletedEvent>()
            .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Id));

        CreateMap<(Reservation reservation, Guid tableId), TableAssignedToReservationEvent>()
            .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.reservation.Id))
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.tableId));

    }
}
