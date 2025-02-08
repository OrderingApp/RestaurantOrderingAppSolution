namespace Domain;

public class Reservation
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public DateTime ReservationDateTime { get; set; }
    public int NumberOfPeople { get; set; }
    public bool IsAssigned { get; set; } = false;

    public Guid? TableId { get; set; }
    public Table? Table { get; set; }
}
