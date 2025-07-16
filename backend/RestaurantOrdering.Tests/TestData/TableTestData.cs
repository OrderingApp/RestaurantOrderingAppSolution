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

    public static Table CreateOngoingTable(Guid? id = null)
    {
        return new Table
        {
            Id = id ?? Guid.NewGuid(),
            Name = "P2",
            Capacity = 4,
            Status = TableStatus.Ongoing,
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
