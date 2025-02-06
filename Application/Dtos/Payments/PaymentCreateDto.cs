using Domain;

namespace Application.Dtos.Payments;

public class PaymentCreateDto
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
}
