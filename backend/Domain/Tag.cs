namespace Domain;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<MenuItemTag> MenuItemTags { get; set; } = new List<MenuItemTag>();
}
