using Application.Dtos.Allergens;
using FluentValidation;

public class AllergenCreateDtoValidator : AbstractValidator<AllergenCreateDto>
{
    public AllergenCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Allergen name is required.")
            .MaximumLength(100)
            .WithMessage("Allergen name must not exceed 100 characters.");

        RuleFor(x => x.EuNumber)
            .InclusiveBetween(1, 14)
            .WithMessage("EU allergen number must be between 1 and 14.")
            .When(x => x.EuNumber.HasValue);
    }
}
