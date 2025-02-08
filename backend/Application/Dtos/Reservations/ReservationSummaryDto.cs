namespace Application.Dtos.Reservations;

public class ReservationSummaryDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public DateTime ReservationDateTime { get; set; }
    public int NumberOfPeople { get; set; }
}
