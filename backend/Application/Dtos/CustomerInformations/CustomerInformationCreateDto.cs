namespace Application.Dtos.CustomerInformations;

public class CustomerInformationCreateDto
{
    public string PhoneNumber { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public string? Address { get; set; }
}
