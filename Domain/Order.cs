namespace Domain;

public class Order
{
    public Guid Id { get; set; }
    public DateTime OrderDateTime { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; } = 0;

    public decimal Discount { get; set; } = 0;

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Ongoing;

    public required OrderType OrderType { get; set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    public Guid? TableId { get; set; }
    public Table? Table { get; set; }

    public Guid? CustomerInformationId { get; set; }
    public CustomerInformation? CustomerInformation { get; set; }
}

public enum OrderStatus
{
    Ongoing,
    Finished,
    Cancelled
}

public enum OrderType
{
    DineIn,
    Takeaway,
    Delivery
}