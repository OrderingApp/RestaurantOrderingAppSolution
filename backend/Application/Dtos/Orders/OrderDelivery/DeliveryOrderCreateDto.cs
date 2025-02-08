using Application.Dtos.OrderItems;

namespace Application.Dtos.Orders.OrderDelivery;

public class DeliveryOrderCreateDto
{
    public DateTime OrderDateTime { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }

    public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
}