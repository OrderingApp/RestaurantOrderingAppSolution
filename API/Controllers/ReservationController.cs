using Application.Contracts;
using Application.Dtos.Reservations;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ReservationController(IReservationService reservationService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateDto reservationCreateDto) =>
        HandleResult(await reservationService.CreateReservation(reservationCreateDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(Guid id) =>
        HandleResult(await reservationService.GetReservation(id));

    [HttpGet]
    public async Task<IActionResult> GetAllReservations() =>
        HandleResult(await reservationService.GetAllReservations());

    [HttpGet("by-date")]
    public async Task<IActionResult> GetReservationsByDate([FromQuery] DateTime date) =>
        HandleResult(await reservationService.GetReservationsByDate(date));

    [HttpPut("{reservationId}/table/{tableId}")]
    public async Task<IActionResult> AssignTableToReservation(Guid reservationId, Guid tableId) =>
        HandleResult(await reservationService.AssignTableToReservation(reservationId, tableId));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation([FromBody] ReservationUpdateDto reservationUpdateDto, Guid id) =>
        HandleResult(await reservationService.UpdateReservation(reservationUpdateDto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(Guid id) =>
        HandleResult(await reservationService.DeleteReservation(id));
}
