using Application.Dtos.Payments;
using FluentValidation;

public class PaymentCreateDtoValidator : AbstractValidator<PaymentCreateDto>
{
    public PaymentCreateDtoValidator()
    {
        RuleFor(p => p.Amount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero.");

        RuleFor(p => p.PaymentMethod).IsInEnum().WithMessage("Invalid payment method specified.");
    }
}
