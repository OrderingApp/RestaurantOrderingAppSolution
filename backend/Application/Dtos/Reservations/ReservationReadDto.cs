namespace Application.Dtos.Reservations;

public class ReservationReadDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime ScheduledFor { get; set; }
    public int CapacityNeeded { get; set; }

    public string? TableName { get; set; }
}
