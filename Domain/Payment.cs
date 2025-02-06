namespace Domain;

public class Payment
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Paid,
    DefferedPayment,
    Cancelled
}

public enum PaymentMethod
{
    Cash,
    Card
}