using Application.Dtos.CustomerInformations;

namespace Application.Dtos.Orders.OrderDelivery;

public class DeliveryOrderSummaryReadDto
{
    public Guid DeliveryOrderId { get; set; }
    public decimal OrderTotal { get; set; }
    public CustomerInformationReadDto? CustomerInformation { get; set; }
}
