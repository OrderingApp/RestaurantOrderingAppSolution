namespace Domain;

public class MenuCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string IconId { get; set; } = "x";
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}