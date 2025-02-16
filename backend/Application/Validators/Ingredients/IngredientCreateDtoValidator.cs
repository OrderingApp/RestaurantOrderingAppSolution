using Application.Dtos.Ingredients;
using FluentValidation;

namespace Application.Validators;

public class IngredientCreateDtoValidator : AbstractValidator<IngredientCreateDto>
{
    public IngredientCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ingredient name is required.")
            .MaximumLength(100).WithMessage("Ingredient name must not exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be a non-negative value.");
    }
}
