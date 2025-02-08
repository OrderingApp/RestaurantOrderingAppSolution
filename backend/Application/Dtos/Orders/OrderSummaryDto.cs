namespace Application.Dtos.Orders;

public class OrderSummaryDto
{
    public Guid Id { get; set; }
    public DateTime OrderDateTime { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string OrderType { get; set; } = null!;
}
