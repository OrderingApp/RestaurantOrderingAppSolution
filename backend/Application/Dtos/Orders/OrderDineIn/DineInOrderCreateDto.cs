using Application.Dtos.OrderItems;

namespace Application.Dtos.Orders.OrderDineIn;

public class DineInOrderCreateDto
{
    public Guid TableId { get; set; }
    public decimal? Discount { get; set; }
    public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
}
