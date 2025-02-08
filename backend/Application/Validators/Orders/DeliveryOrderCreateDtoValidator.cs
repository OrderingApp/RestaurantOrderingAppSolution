using Application.Dtos.Orders.OrderDelivery;
using FluentValidation;

public class DeliveryOrderCreateDtoValidator : AbstractValidator<DeliveryOrderCreateDto>
{
    public DeliveryOrderCreateDtoValidator()
    {
        RuleFor(x => x.OrderDateTime)
            .NotEmpty().WithMessage("OrderDateTime is required.")
            .Must(BeAValidDate).WithMessage("OrderDateTime must be in the future.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .Matches(@"^\d{9,15}$").WithMessage("PhoneNumber must be between 9 and 15 digits.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
