namespace Domain;

public class Area
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
    public List<Table> Tables { get; set; } = new();
}
