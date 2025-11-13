using Application.Dtos.Orders.OrderTakeAway;
using Application.Validators;
using Application.Validators.CustomerInformation;
using FluentValidation;

public class TakeawayOrderCreateDtoValidator : AbstractValidator<TakeawayOrderCreateDto>
{
    public TakeawayOrderCreateDtoValidator()
    {
        RuleFor(x => x.CustomerInformation)
            .NotNull()
            .WithMessage("CustomerInformation is required.")
            .SetValidator(new CustomerInformationCreateDtoValidator());

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());
    }
}
