namespace Domain;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}

public class Order : AuditableEntity
{
    public Guid Id { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal? DeliveryPrice { get; set; }

    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => Math.Max(0, TotalAmount - PaidAmount);

    public OrderStatus Status { get; set; } = OrderStatus.Ongoing;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
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
    Completed,
    Closed,
    Cancelled,
}

public enum PaymentStatus
{
    Unpaid,
    PartiallyPaid,
    Paid,
}

public enum OrderType
{
    DineIn,
    Takeaway,
    Delivery,
}
