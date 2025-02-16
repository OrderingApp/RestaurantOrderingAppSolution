using Application.Dtos.Orders.OrderDineIn;
using Application.Validators;
using FluentValidation;

public class DineInOrderCreateDtoValidator : AbstractValidator<DineInOrderCreateDto>
{
    public DineInOrderCreateDtoValidator()
    {
        RuleFor(x => x.OrderDateTime)
            .NotEmpty().WithMessage("OrderDateTime is required.")
            .Must(BeAValidDate).WithMessage("OrderDateTime must be in the future.");

        RuleFor(x => x.TableId)
            .NotEmpty().WithMessage("TableId is required.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
