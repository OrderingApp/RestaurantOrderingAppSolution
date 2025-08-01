using Application.Dtos.CustomerInformations;
using Application.Dtos.OrderItems;

namespace Application.Dtos.Orders.OrderDelivery;

public class DeliveryOrderCreateDto
{
    public DateTime CreatedAt { get; set; }
    public decimal? Discount { get; set; }
    public CustomerInformationCreateDto CustomerInformation { get; set; } = null!;

    public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
}
