using Application.Dtos.Orders.OrderTakeAway;
using Application.Validators;
using Application.Validators.CustomerInformation;
using FluentValidation;

public class TakeawayOrderCreateDtoValidator : AbstractValidator<TakeawayOrderCreateDto>
{
    public TakeawayOrderCreateDtoValidator()
    {
        RuleFor(x => x.DateTime)
            .NotEmpty()
            .WithMessage("OrderDateTime is required.")
            .Must(BeAValidDate)
            .WithMessage("OrderDateTime must be in the future.");

        RuleFor(x => x.CustomerInformation)
            .NotNull()
            .WithMessage("CustomerInformation is required.")
            .SetValidator(new CustomerInformationCreateDtoValidator());

        RuleForEach(x => x.OrderItems).SetValidator(new OrderItemCreateDtoValidator());
    }

    private bool BeAValidDate(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
