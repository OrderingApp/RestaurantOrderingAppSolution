using Domain;

namespace Application.Dtos.Orders.OrderDelivery;

public class DeliveryOrderReadDto
{
    public Guid DeliveryOrderId { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string? AdditionalInstructions { get; set; }
    public string Address { get; set; } = null!;
    public DateTime? ExpectedOrderCompletion { get; set; }

    public OrderCompletionType OrderCompletionType { get; set; } = OrderCompletionType.Immediate;
    public PreferedPaymentMethod PreferedPaymentMethod { get; set; }
}
