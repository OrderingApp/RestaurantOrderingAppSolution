using Domain;

namespace Application.Dtos.Tables;

public class TableUpdateDto
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public bool? IsUsed { get; set; }
    public TableStatus? Status { get; set; }
}
