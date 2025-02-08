using Application.Dtos.Reservations;

namespace Application.Dtos.Tables;

public class TableReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int NumberOfPeople { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsUsed { get; set; }

    public ReservationSummaryDto? Reservation { get; set; }
}
