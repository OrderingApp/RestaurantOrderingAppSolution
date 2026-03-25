using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class ReservationSeedModel
{
    public Guid Id { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Name { get; set; }
    public DateTime ScheduledFor { get; set; }
    public int CapacityNeeded { get; set; }
    public bool IsAssigned { get; set; }
    public Guid? TableId { get; set; }
}
