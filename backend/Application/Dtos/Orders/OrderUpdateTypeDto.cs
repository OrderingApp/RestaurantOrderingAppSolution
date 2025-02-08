namespace Application.Dtos.Orders;

public class OrderUpdateTypeDto
{
    public Guid? TableId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? AdditionalInstructions { get; set; }
}
