namespace Application.Dtos.Orders;

public class NonDineInOrderSummaryDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpectedOrderCompletion { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string OrderType { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}
