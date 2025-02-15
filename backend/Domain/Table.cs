namespace Domain;

public class Table
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }

    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<Order> Orders { get; set; } = new();

    public List<Reservation> Reservations { get; set; } = new();

    public TableStatus TableStatus { get; set; } = TableStatus.Available;
}

public enum TableStatus
{
    Available,
    Reserved,
    Ongoing,
    PendingServingOrderItems,
    OrderItemsServed,
    PendingPayment,
    Closed
}
