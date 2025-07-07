using Domain;

public static class TableTestData
{
    public static Table CreateCorrectTable(Guid? id = null)
    {
        return new Table
        {
            Id = id ?? Guid.NewGuid(),
            Name = "P1",
            Capacity = 8,
            Status = TableStatus.Available,
        };
    }

    public static Table CreateIncorrectTable(Guid? id = null)
    {
        return new Table
        {
            Id = id ?? Guid.NewGuid(),
            Name = "",
            Capacity = 8,
            Status = TableStatus.Available,
        };
    }
}
