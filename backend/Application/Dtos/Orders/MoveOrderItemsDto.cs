namespace Application.Dtos.Orders;

public class MoveOrderItemsDto
{
    public List<SplitOrderGroupDto> SplitGroups { get; set; } = new();
}
