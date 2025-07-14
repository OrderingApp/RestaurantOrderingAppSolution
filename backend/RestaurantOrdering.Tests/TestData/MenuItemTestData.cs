using Domain;

public static class MenuItemTestData
{
    public const string MenuItem = "Pizza";
    public static MenuItem CreateMenuItem(Guid? id = null, string? name = null, decimal price = 10m)
    {
        return new MenuItem
        {
            Id = id ?? Guid.NewGuid(),
            Name = name ?? MenuItem,
            Price = price,
        };
    }
}
