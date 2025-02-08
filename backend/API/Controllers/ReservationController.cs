using Application.Contracts;
using Application.Dtos.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages table reservations.
/// </summary>
/// 
public class ReservationController(IReservationService reservationService) : BaseApiController
{
    /// <summary>
    /// Creates a new reservation.
    /// </summary>
    /// <param name="reservationCreateDto">The reservation details.</param>
    /// <returns>The created reservation.</returns>
    /// <response code="201">If the reservation was successfully created.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto reservationCreateDto) =>
        HandleResult(await reservationService.CreateReservation(reservationCreateDto));

    /// <summary>
    /// Retrieves a specific reservation by ID.
    /// </summary>
    /// <param name="id">The unique reservation ID.</param>
    /// <returns>The requested reservation.</returns>
    /// <response code="200">Returns the reservation.</response>
    /// <response code="404">If the reservation is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetReservation(Guid id) =>
        HandleResult(await reservationService.GetReservation(id));

    /// <summary>
    /// Retrieves all reservations.
    /// </summary>
    /// <returns>A list of reservations.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllReservations() =>
        HandleResult(await reservationService.GetAllReservations());

    /// <summary>
    /// Retrieves reservations for a specific date.
    /// </summary>
    /// <param name="date">The reservation date to filter by.</param>
    /// <returns>A list of reservations on the specified date.</returns>
    [HttpGet("by-date")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetReservationsByDate([FromQuery] DateTime date) =>
        HandleResult(await reservationService.GetReservationsByDate(date));

    /// <summary>
    /// Assigns a table to a reservation.
    /// </summary>
    /// <param name="reservationId">The reservation ID.</param>
    /// <param name="tableId">The table ID to assign.</param>
    /// <returns>The updated reservation with the assigned table.</returns>
    /// <response code="200">If the table was assigned successfully.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the reservation or table is not found.</response>
    [HttpPut("{reservationId}/table/{tableId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AssignTableToReservation(Guid reservationId, Guid tableId) =>
        HandleResult(await reservationService.AssignTableToReservation(reservationId, tableId));

    /// <summary>
    /// Updates a reservation.
    /// </summary>
    /// <param name="id">The ID of the reservation to update.</param>
    /// <param name="reservationUpdateDto">The updated reservation details.</param>
    /// <returns>The updated reservation.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the reservation is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateReservation([FromBody] ReservationUpdateDto reservationUpdateDto, Guid id) =>
        HandleResult(await reservationService.UpdateReservation(reservationUpdateDto, id));

    /// <summary>
    /// Deletes a reservation.
    /// </summary>
    /// <param name="id">The reservation ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the reservation is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteReservation(Guid id) =>
        HandleResult(await reservationService.DeleteReservation(id));
}
