using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class OrderItemSeedModel
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public string? SpecialInstructions { get; set; }
    public OrderItemStatus Status { get; set; }
    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }
}
