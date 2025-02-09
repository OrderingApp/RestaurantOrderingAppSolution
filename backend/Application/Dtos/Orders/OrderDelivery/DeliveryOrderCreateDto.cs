using Application.Dtos.OrderItems;
using Domain;

namespace Application.Dtos.Orders.OrderDelivery;

public class DeliveryOrderCreateDto
{
    public DateTime OrderDateTime { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; } = OrderCompletionType.Immediate;
    public PreferedPaymentMethod PreferedPaymentMethod { get; set; }
    public DateTime? ExpectedOrderCompletion { get; set; }

    public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
}
