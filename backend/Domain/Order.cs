namespace Domain;

public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}

public class Order : AuditableEntity
{
    public Guid Id { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal? DeliveryPrice { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Ongoing;
    public OrderType Type { get; set; }

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
    Cancelled,
}

public enum OrderType
{
    DineIn,
    Takeaway,
    Delivery,
}
