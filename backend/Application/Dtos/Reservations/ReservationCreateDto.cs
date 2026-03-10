namespace Application.Dtos.Reservations;

public class ReservationCreateDto
{
    public string PhoneNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? ScheduledFor { get; set; }
    public int CapacityNeeded { get; set; }

    public Guid? TableId { get; set; }
}
