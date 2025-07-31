using Application.Dtos.Orders.OrderDelivery;
using Application.Validators;
using Application.Validators.CustomerInformation;
using FluentValidation;

public class DeliveryOrderCreateDtoValidator : AbstractValidator<DeliveryOrderCreateDto>
{
    public DeliveryOrderCreateDtoValidator()
    {
        RuleFor(x => x.CreatedAt)
            .NotEmpty()
            .WithMessage("Order date and time is required.")
            .Must(BeAValidDate)
            .WithMessage("Order date and time must be in the future.");

        RuleFor(x => x.CustomerInformation)
            .NotNull()
            .WithMessage("Customer information is required.")
            .SetValidator(new CustomerInformationCreateDtoValidator());

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
