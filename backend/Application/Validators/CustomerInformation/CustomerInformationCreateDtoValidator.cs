using Application.Dtos.CustomerInformations;
using Domain;
using FluentValidation;

namespace Application.Validators.CustomerInformation;

public class CustomerInformationCreateDtoValidator : AbstractValidator<CustomerInformationCreateDto>
{
    public CustomerInformationCreateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{8,14}$")
            .WithMessage("Invalid phone number format.")
            .Length(9, 15)
            .WithMessage("Phone number must be between 9 and 15 digits.");

        RuleFor(x => x.OrderCompletionType)
            .IsInEnum()
            .WithMessage("Invalid order completion type.");

        RuleFor(x => x.PreferredPaymentMethod)
            .IsInEnum()
            .WithMessage("Invalid preferred payment method.");

        RuleFor(x => x.ExpectedOrderCompletion)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expected completion must be in the future.")
            .When(x => x.ExpectedOrderCompletion.HasValue);
    }
}
