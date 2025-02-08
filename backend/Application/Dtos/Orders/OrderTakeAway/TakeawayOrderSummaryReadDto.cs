using Domain;

namespace Application.Dtos.Orders.OrderTakeAway;

public class TakeawayOrderSummaryReadDto
{
    public Guid TakeawayOrderId { get; set; }
    public string PhoneNumber { get; set; } = null!;

    public string? AdditionalInstructions { get; set; }
    public DateTime? ExpectedOrderCompletion { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; } = OrderCompletionType.Immediate;
    public PreferedPaymentMethod PreferedPaymentMethod { get; set; }
}
