using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class TagSeedModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
}
