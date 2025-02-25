namespace Application.Dtos.Reservations;

public class ReservationSummaryDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public int CapacityNeeded { get; set; }
}
