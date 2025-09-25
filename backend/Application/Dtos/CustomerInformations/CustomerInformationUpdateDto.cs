using Domain;

namespace Application.Dtos.CustomerInformations;

public class CustomerInformationUpdateDto
{
    public string PhoneNumber { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public string? Address { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; }
    public DateTime? ExpectedOrderCompletion { get; set; }
}
