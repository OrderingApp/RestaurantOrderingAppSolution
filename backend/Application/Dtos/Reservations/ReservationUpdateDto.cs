namespace Application.Dtos.Reservations;

public class ReservationUpdateDto
{
    public DateTime? DateTime { get; set; }
    public int? CapacityNeeded { get; set; }
    public Guid? TableId { get; set; }
}
