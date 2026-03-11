using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class OrderSeedModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal? DeliveryPrice { get; set; }
    public OrderStatus Status { get; set; }
    public OrderType Type { get; set; }
    public Guid? TableId { get; set; }
    public Guid? CustomerInformationId { get; set; }
}
