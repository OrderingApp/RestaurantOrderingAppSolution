using Application.Dtos.Allergens;
using FluentValidation;

public class AllergenUpdateDtoValidator : AbstractValidator<AllergenUpdateDto>
{
    public AllergenUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Allergen name is required.")
            .MaximumLength(100)
            .WithMessage("Allergen name must not exceed 100 characters.");

        RuleFor(x => x.IsUsed).NotNull().WithMessage("IsUsed flag must be specified.");

        RuleFor(x => x.EuNumber)
            .InclusiveBetween(1, 14)
            .WithMessage("EU allergen number must be between 1 and 14.")
            .When(x => x.EuNumber.HasValue);
    }
}
