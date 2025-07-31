namespace Domain;

public class Reservation
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ScheduledFor { get; set; }
    public int CapacityNeeded { get; set; }
    public bool IsAssigned { get; set; } = false;

    public Guid? TableId { get; set; }
    public Table? Table { get; set; }
}
