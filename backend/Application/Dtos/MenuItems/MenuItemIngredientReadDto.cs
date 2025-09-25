namespace Application.Dtos.MenuItems;

public class MenuItemIngredientWithTagsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Guid> TagIds { get; set; } = new();
}

public class MenuItemIngredientBasicDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
