namespace Domain;

public class SubCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int SequenceNumber { get; set; }

    public Guid MenuCategoryId { get; set; }
    public List<MenuItem> MenuItems { get; set; } = new();
}
