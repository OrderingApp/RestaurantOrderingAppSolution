namespace Application.Dtos.Orders;

public class SplitOrderDto
{
    public List<Guid> OrderItemIds { get; set; } = new List<Guid>();
}
