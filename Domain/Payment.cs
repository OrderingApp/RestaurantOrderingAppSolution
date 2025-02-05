namespace Domain;

public class Payment
{
    public Guid Id { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public PaymentMethod? PaymentMethod { get; set; }

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