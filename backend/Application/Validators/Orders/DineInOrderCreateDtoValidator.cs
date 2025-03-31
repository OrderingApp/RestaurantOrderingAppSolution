using Application.Dtos.Orders.OrderDineIn;
using Application.Validators;
using FluentValidation;

public class DineInOrderCreateDtoValidator : AbstractValidator<DineInOrderCreateDto>
{
    public DineInOrderCreateDtoValidator()
    {
        RuleFor(x => x.DateTime)
            .NotEmpty()
            .WithMessage("Order date and time is required.")
            .Must(BeAValidDate)
            .WithMessage("Order date and time must be in the future.");

        RuleFor(x => x.TableId)
            .NotEmpty()
            .WithMessage("Table ID is required.")
            .NotEqual(Guid.Empty)
            .WithMessage("Table ID must be a valid GUID.");

        RuleFor(x => x.OrderItems).NotEmpty().WithMessage("At least one order item is required.");

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
