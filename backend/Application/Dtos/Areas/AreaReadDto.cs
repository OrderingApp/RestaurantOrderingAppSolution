using Application.Dtos.Tables;

namespace Application.Dtos.Areas;

public class AreaReadDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int SequenceNumber { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
    public List<TableReadDto> Tables { get; set; } = new();
}
