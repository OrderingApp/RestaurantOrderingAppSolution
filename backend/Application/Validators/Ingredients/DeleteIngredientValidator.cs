using FluentValidation;

namespace Application.Validators;

public class DeleteIngredientValidator : AbstractValidator<Guid>
{
    public DeleteIngredientValidator()
    {
        RuleFor(id => id)
            .NotEmpty().WithMessage("Ingredient ID is required.");
    }
}
