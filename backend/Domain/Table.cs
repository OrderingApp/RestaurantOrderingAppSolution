namespace Domain;

public class Table
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int NumberOfPeople { get; set; }
    public bool IsOccupied { get; set; }

    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<Order> Orders { get; set; } = new List<Order>();

    public List<Reservation> Reservations { get; set; } = new List<Reservation>();
}
