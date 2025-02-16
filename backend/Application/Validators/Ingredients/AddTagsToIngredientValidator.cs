using FluentValidation;

namespace Application.Validators;

public class AddTagsToIngredientValidator : AbstractValidator<List<Guid>>
{
    public AddTagsToIngredientValidator()
    {
        RuleFor(tagIds => tagIds)
            .NotEmpty().WithMessage("At least one tag ID is required.");

        RuleForEach(tagIds => tagIds)
            .NotEmpty().WithMessage("Each tag ID must be a valid GUID.");
    }
}
