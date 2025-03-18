namespace Domain;

public class MenuCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int SequenceNumber { get; set; }

    public List<SubCategory> SubCategories { get; set; } = new();
    public List<MenuItem> MenuItems { get; set; } = new();
}