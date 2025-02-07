using Application.Dtos.Orders.OrderTakeAway;
using FluentValidation;

public class TakeawayOrderCreateDtoValidator : AbstractValidator<TakeawayOrderCreateDto>
{
    public TakeawayOrderCreateDtoValidator()
    {
        RuleFor(x => x.OrderDateTime)
            .NotEmpty().WithMessage("OrderDateTime is required.")
            .Must(BeAValidDate).WithMessage("OrderDateTime must be in the future.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.")
            .Matches(@"^\d{9,15}$").WithMessage("PhoneNumber must be between 9 and 15 digits.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
