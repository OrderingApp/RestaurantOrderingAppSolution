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
    }
}
