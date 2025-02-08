using Application.Dtos.Orders;

namespace Application.Dtos.Tables;

public class TableSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int NumberOfPeople { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsUsed { get; set; }

    public List<OrderSummaryDto> Orders { get; set; } = new List<OrderSummaryDto>();
}
