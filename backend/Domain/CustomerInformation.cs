namespace Domain;

public class CustomerInformation
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public string? Address { get; set; } 
    public DateTime? ExpectedOrderCompletion { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; } = OrderCompletionType.Immediate;
    public PreferedPaymentMethod PreferedPaymentMethod { get; set; }

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }
}

public enum OrderCompletionType
{
    Immediate,
    Scheduled
}

public enum PreferedPaymentMethod
{
    Card,
    Cash
}