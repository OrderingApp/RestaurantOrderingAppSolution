using Application.Dtos.Orders.OrderTakeAway;
using Application.Validators;
using FluentValidation;

public class TakeawayOrderCreateDtoValidator : AbstractValidator<TakeawayOrderCreateDto>
{
    public TakeawayOrderCreateDtoValidator()
    {
        RuleFor(x => x.OrderDateTime)
            .NotEmpty().WithMessage("OrderDateTime is required.")
            .Must(BeAValidDate).WithMessage("OrderDateTime must be in the future.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
