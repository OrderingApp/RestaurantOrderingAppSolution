using System;
using Domain;

namespace Infrastructure.Database.Seeder.SeedModels;

public class CustomerInformationSeedModel
{
    public Guid Id { get; set; }
    public required string PhoneNumber { get; set; }
    public string? AdditionalInstructions { get; set; }
    public string? Address { get; set; }
    public DateTime? ExpectedOrderCompletion { get; set; }
    public OrderCompletionType OrderCompletionType { get; set; }
    public PaymentMethod? PreferredPaymentMethod { get; set; }
    public Guid OrderId { get; set; }
}
