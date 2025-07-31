using System.Net;
using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.Reservations;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Reservations;

namespace Application.Services;

public class ReservationService(
    RestaurantOrderingContext orderingContext,
    IEventHandlerService eventHandlerService,
    IMapper mapper
) : IReservationService
{
    public async Task<ResultDto<ReservationReadDto>> CreateReservation(
        ReservationCreateDto reservationCreate
    )
    {
        try
        {
            var reservation = mapper.Map<Reservation>(reservationCreate);

            await orderingContext.Reservations.AddAsync(reservation);
            await orderingContext.SaveChangesAsync();

            var createdReservation = mapper.Map<ReservationReadDto>(reservation);

            var reservationCreatedEvent = mapper.Map<ReservationCreatedEvent>(reservation);
            await eventHandlerService.HandleEventAsync(reservationCreatedEvent);

            return ResultDto<ReservationReadDto>.Success(
                createdReservation,
                HttpStatusCode.Created
            );
        }
        catch (Exception ex)
        {
            return ResultDto<ReservationReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<ReservationReadDto>> GetReservation(Guid id)
    {
        try
        {
            var reservation = await orderingContext
                .Reservations.Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return ResultDto<ReservationReadDto>.Failure(
                    "Reservation not found.",
                    HttpStatusCode.NotFound
                );

            var reservationDto = mapper.Map<ReservationReadDto>(reservation);

            return ResultDto<ReservationReadDto>.Success(reservationDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<ReservationReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<List<ReservationReadDto>>> GetReservationsByDate(DateTime date)
    {
        try
        {
            var reservations = await orderingContext
                .Reservations.Where(r => r.ScheduledFor.Date == date.Date)
                .Include(r => r.Table)
                .ToListAsync();

            var reservationDtos = mapper.Map<List<ReservationReadDto>>(reservations);

            return ResultDto<List<ReservationReadDto>>.Success(reservationDtos, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<List<ReservationReadDto>>.Failure(
                $"An error occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    // fix name it should be assign reservation to table not reverse
    public async Task<ResultDto<ReservationReadDto>> AssignTableToReservation(Guid id, Guid tableId)
    {
        try
        {
            var reservation = await orderingContext.Reservations.FirstOrDefaultAsync(r =>
                r.Id == id
            );

            if (reservation == null)
                return ResultDto<ReservationReadDto>.Failure(
                    "Reservation not found.",
                    HttpStatusCode.NotFound
                );

            var table = await orderingContext.Tables.FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
                return ResultDto<ReservationReadDto>.Failure(
                    "Table not found.",
                    HttpStatusCode.NotFound
                );

            reservation.IsAssigned = true;
            reservation.TableId = tableId;

            await orderingContext.SaveChangesAsync();

            var tableAssignedEvent = mapper.Map<TableAssignedToReservationEvent>(
                (reservation, tableId)
            );
            await eventHandlerService.HandleEventAsync(tableAssignedEvent);

            var updatedReservationDto = mapper.Map<ReservationReadDto>(reservation);

            return ResultDto<ReservationReadDto>.Success(updatedReservationDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<ReservationReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<ReservationReadDto>> UpdateReservation(
        Guid id,
        ReservationUpdateDto reservationUpdate
    )
    {
        try
        {
            var reservation = await orderingContext.Reservations.FindAsync(id);

            if (reservation == null)
                return ResultDto<ReservationReadDto>.Failure(
                    "Reservation not found.",
                    HttpStatusCode.NotFound
                );

            mapper.Map(reservationUpdate, reservation);

            await orderingContext.SaveChangesAsync();

            var updatedReservationDto = mapper.Map<ReservationReadDto>(reservation);

            var reservationUpdatedEvent = mapper.Map<ReservationUpdatedEvent>(reservation);
            await eventHandlerService.HandleEventAsync(reservationUpdatedEvent);

            return ResultDto<ReservationReadDto>.Success(updatedReservationDto, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return ResultDto<ReservationReadDto>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ResultDto<bool>> DeleteReservation(Guid id)
    {
        try
        {
            var reservation = await orderingContext.Reservations.FirstOrDefaultAsync(r =>
                r.Id == id
            );

            if (reservation == null)
                return ResultDto<bool>.Failure("Reservation not found.", HttpStatusCode.NotFound);

            orderingContext.Reservations.Remove(reservation);
            await orderingContext.SaveChangesAsync();

            var reservationDeletedEvent = mapper.Map<ReservationDeletedEvent>(reservation);
            await eventHandlerService.HandleEventAsync(reservationDeletedEvent);

            return ResultDto<bool>.Success(true, HttpStatusCode.NoContent);
        }
        catch (Exception ex)
        {
            return ResultDto<bool>.Failure(
                $"An error occured: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}
