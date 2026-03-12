using Application.Dtos.IngredientCategories;
using FluentValidation;

public class IngredientCategoryCreateDtoValidator : AbstractValidator<IngredientCategoryCreateDto>
{
    public IngredientCategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Ingredient category name is required.")
            .MaximumLength(50)
            .WithMessage("Ingredient category name must not exceed 50 characters.");
    }
}
