using Application.Dtos.Reservations;
using Domain;

namespace Application.Dtos.Tables;

public class TableReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }

    public ReservationSummaryDto? Reservation { get; set; }
    public TableStatus Status { get; set; }
}
