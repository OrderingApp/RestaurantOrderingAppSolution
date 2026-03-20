using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class PaymentSeedModel
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }
    public bool IsRefunded { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public Guid OrderId { get; set; }
}
