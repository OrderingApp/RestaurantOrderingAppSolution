using Application.Dtos.CustomerInformations;
using FluentValidation;

namespace Application.Validators;

public class CustomerInformationUpdateDtoValidator : AbstractValidator<CustomerInformationUpdateDto>
{
    public CustomerInformationUpdateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d+$").WithMessage("Phone number must contain only digits.")
            .Length(9, 15).WithMessage("Phone number must be between 9 and 15 digits.");

        RuleFor(x => x.Address)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address cannot be empty if provided.");

        RuleFor(x => x.AdditionalInstructions)
            .MaximumLength(250).WithMessage("Additional instructions must not exceed 250 characters.");
    }
}