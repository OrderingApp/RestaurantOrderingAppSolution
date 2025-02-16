using Domain;

namespace Application.Dtos.CustomerInformations;

public class CustomerInformationCreateDto
{
    public string PhoneNumber { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public string? Address { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; }
    public PreferredPaymentMethod PreferredPaymentMethod { get; set; }
    public DateTime? ExpectedOrderCompletion { get; set; }
}
