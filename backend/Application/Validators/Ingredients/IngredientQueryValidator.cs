using FluentValidation;

namespace Application.Validators;

public class IngredientQueryValidator : AbstractValidator<List<string>>
{
    public IngredientQueryValidator()
    {
        RuleFor(tags => tags)
            .NotEmpty().When(tags => tags != null)
            .WithMessage("Tags cannot be empty.");

        RuleForEach(tags => tags)
            .NotEmpty().WithMessage("Each tag must be a valid non-empty string.");
    }
}
