using Domain;

namespace Application.Dtos.Payments;

public class PaymentReadDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime? PaidAt { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public Guid OrderId { get; set; }
}