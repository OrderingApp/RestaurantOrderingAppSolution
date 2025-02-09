namespace Domain;

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDateTime { get; set; } = DateTime.UtcNow;

    public decimal TotalAmount { get; set; } = 0;
    public decimal Discount { get; set; } = 0;

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Ongoing;

    public OrderType OrderType { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    public Guid? TableId { get; set; }
    public Table? Table { get; set; }

    public Guid? CustomerInformationId { get; set; }
    public CustomerInformation? CustomerInformation { get; set; }

    public List<Payment> Payments { get; set; } = new();
}

public enum OrderStatus
{
    Ongoing,
    PendingPayment,
    Closed,
    Cancelled
}

public enum OrderType
{
    DineIn,
    Takeaway,
    Delivery
}