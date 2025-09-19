using Application.Dtos.Orders.OrderDelivery;
using Application.Validators;
using Application.Validators.CustomerInformation;
using FluentValidation;

public class DeliveryOrderCreateDtoValidator : AbstractValidator<DeliveryOrderCreateDto>
{
    public DeliveryOrderCreateDtoValidator()
    {
        RuleFor(x => x.CustomerInformation)
            .NotNull()
            .WithMessage("Customer information is required.")
            .SetValidator(new CustomerInformationCreateDtoValidator());

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());
    }
}
