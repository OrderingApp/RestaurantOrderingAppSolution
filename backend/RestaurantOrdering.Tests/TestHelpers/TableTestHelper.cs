using Domain;
using System;

public static class TableTestHelper
{
    public static Table CreateTable(Guid? id = null)
    {
        return new Table
        {
            Id = id ?? Guid.NewGuid(),
            Name = "P1",
            Capacity = 8,
            Status = TableStatus.Available
        };
    }
}
