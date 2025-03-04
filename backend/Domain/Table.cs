namespace Domain;

public class Table
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }

    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public Guid AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public List<Order> Orders { get; set; } = new();

    public List<Reservation> Reservations { get; set; } = new();

    public TableStatus Status { get; set; } = TableStatus.Available;
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
