using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class MenuItemSeedModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsUsed { get; set; }
    public bool IsDeleted { get; set; }
    public Guid? MenuCategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public int SequenceNumber { get; set; }
}
