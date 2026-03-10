namespace Application.Dtos.Tables;

public class TableCreateDto
{
    public string Name { get; set; } = null!;
    public int Capacity { get; set; }
    public int SequenceNumber { get; set; }
    public Guid AreaId { get; set; }
}
