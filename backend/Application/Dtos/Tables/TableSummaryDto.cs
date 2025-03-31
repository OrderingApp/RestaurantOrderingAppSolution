using Application.Dtos.Orders;
using Domain;

namespace Application.Dtos.Tables;

public class TableSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }

    public List<OrderSummaryDto> Orders { get; set; } = new List<OrderSummaryDto>();
    public TableStatus Status { get; set; }
}
