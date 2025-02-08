using Application.Dtos.Common;
using Application.Dtos.Reservations;

namespace Application.Contracts;

public interface IReservationService
{
    Task<ResultDto<ReservationReadDto>> CreateReservation(ReservationCreateDto reservationCreate);
    Task<ResultDto<ReservationReadDto>> GetReservation(Guid id);
    Task<ResultDto<List<ReservationReadDto>>> GetAllReservations();
    Task<ResultDto<List<ReservationReadDto>>> GetReservationsByDate(DateTime date);
    Task<ResultDto<ReservationReadDto>> UpdateReservation(ReservationUpdateDto reservationUpdate, Guid id);
    Task<ResultDto<ReservationReadDto>> AssignTableToReservation(Guid reservationId, Guid tableId);
    Task<ResultDto<bool>> DeleteReservation(Guid id);
}
