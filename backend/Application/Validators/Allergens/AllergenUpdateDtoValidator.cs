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
    }
}
