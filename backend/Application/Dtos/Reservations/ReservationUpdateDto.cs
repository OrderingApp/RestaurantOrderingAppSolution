namespace Application.Dtos.Reservations;

public class ReservationUpdateDto
{
    public string? PhoneNumber { get; set; }
    public string? Name { get; set; }
    public DateTime? DateTime { get; set; }
    public int? CapacityNeeded { get; set; }
    public Guid? TableId { get; set; }
}
