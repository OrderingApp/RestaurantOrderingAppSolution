using Application.Dtos.Reservations;
using Domain;

namespace Application.Dtos.Tables;

public class TableReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int SequenceNumber { get; set; }
    public bool IsPrepared { get; set; }
    public DateTime? ActiveSince { get; set; }

    public List<ReservationReadDto> Reservations { get; set; } = new();
    public TableStatus Status { get; set; }
}
