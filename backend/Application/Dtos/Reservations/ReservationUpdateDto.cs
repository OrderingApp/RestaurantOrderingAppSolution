namespace Application.Dtos.Reservations;

public class ReservationUpdateDto
{
    public DateTime? ReservationDateTime { get; set; }
    public int? NumberOfPeople { get; set; }
    public Guid? TableId { get; set; }
}
