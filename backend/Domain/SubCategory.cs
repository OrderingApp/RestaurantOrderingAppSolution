namespace Domain;

public class SubCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }

    public Guid MenuCategoryId { get; set; }
    public MenuCategory MenuCategory { get; set; } = null!;

    public List<MenuItem> MenuItems { get; set; } = new();
}
