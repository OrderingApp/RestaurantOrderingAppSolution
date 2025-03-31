namespace Domain;

public class Payment
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    public bool IsRefunded { get; set; } = false;

    public PaymentMethod PaymentMethod { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
}

public enum PaymentMethod
{
    Cash,
    Card,
}
