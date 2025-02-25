using Application.Dtos.CustomerInformations;
using Application.Dtos.OrderItems;

namespace Application.Dtos.Orders.OrderTakeAway;

public class TakeawayOrderCreateDto
{
    public DateTime DateTime { get; set; }
    public CustomerInformationCreateDto CustomerInformation { get; set; } = null!;
    public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
}
