namespace Application.Dtos.Orders;

public class SplitOrderGroupDto
{
    public List<Guid> OrderItemIds { get; set; } = new();
}
