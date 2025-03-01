using Application.Dtos.Ingredients;
using FluentValidation;

namespace Application.Validators;

public class IngredientUpdateDtoValidator : AbstractValidator<IngredientUpdateDto>
{
    public IngredientUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null)
            .WithMessage("Ingredient name cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Ingredient name must not exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Price.HasValue)
            .WithMessage("Price must be a non-negative value.");


        RuleFor(x => x.IsUsed)
            .NotNull().When(x => x.IsUsed.HasValue)
            .WithMessage("IsUsed must be true or false if provided.");

    }
}
