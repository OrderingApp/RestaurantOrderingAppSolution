using Application.Dtos.CustomerInformations;
using Application.Dtos.OrderItems;

namespace Application.Dtos.Orders;

public class OrderReadDto
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string OrderStatus { get; set; } = null!;
    public string OrderType { get; set; } = null!;

    public Guid? TableId { get; set; }
    public CustomerInformationReadDto? CustomerInformation { get; set; }

    public List<OrderItemReadDto> OrderItems { get; set; } = new();
}
