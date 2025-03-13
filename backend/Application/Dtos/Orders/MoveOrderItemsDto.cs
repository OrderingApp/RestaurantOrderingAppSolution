namespace Application.Dtos.Orders;

public class MoveOrderItemsDto
{
    public List<Guid> OrderItemIds { get; set; } = new();
}
