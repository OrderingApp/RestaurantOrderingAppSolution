using Domain;

public static class MenuItemTestData
{
    public static MenuItem CreateMenuItem(Guid? id = null, decimal price = 10m)
    {
        return new MenuItem
        {
            Id = id ?? Guid.NewGuid(),
            Price = price,
            Name = "Test Menu Item",
        };
    }
}
