using Application.Dtos.OrderItems;
using Application.Dtos.Payments;
using Domain;

namespace Application.Dtos.Orders;

public class OrderCloseDto
{
    public Guid Id { get; set; }
    public decimal TotalAmount { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public OrderStatus OrderStatus { get; set; }
    public List<OrderItemReadDto> OrderItems { get; set; } = new();
    public List<PaymentReadDto> Payments { get; set; } = new();
}
