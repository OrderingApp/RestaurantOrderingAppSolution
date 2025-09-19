namespace Domain;

public class CustomerInformation
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public string? Address { get; set; }

    public DateTime? ExpectedOrderCompletion { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; } = OrderCompletionType.Immediate;
    public PreferredPaymentMethod PreferredPaymentMethod { get; set; } // we dont actually need this

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
}

public enum OrderCompletionType
{
    Immediate,
    Scheduled,
}

public enum PreferredPaymentMethod
{
    Card,
    Cash,
}
