using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class TableSeedModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
    public int SequenceNumber { get; set; }
    public bool IsPrepared { get; set; }
    public DateTime? ActiveSince { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
    public TableStatus Status { get; set; }
    public Guid AreaId { get; set; }
}
