using Application.Dtos.Orders.OrderDelivery;
using FluentValidation;

public class DeliveryOrderCreateDtoValidator : AbstractValidator<DeliveryOrderCreateDto>
{
    public DeliveryOrderCreateDtoValidator()
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
