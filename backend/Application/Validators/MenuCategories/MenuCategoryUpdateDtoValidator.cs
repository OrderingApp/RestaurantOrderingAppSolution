using Application.Dtos.MenuCategories;
using FluentValidation;

namespace Application.Validators.MenuCategories;

public class MenuCategoryUpdateDtoValidator : AbstractValidator<MenuCategoryUpdateDto>
{
    public MenuCategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .When(x => x.Name != null)
            .WithMessage("Name cannot be empty if provided.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MinimumLength(2)
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Name must be at least 2 characters.")
            .Must(name => name == null || !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name cannot be empty or only spaces.");

        RuleFor(x => x.IsUsed)
            .NotNull()
            .When(x => x.IsUsed != null)
            .WithMessage("IsUsed must be true or false if provided.");

        RuleFor(x => x.IsDeleted)
            .NotNull()
            .When(x => x.IsDeleted != null)
            .WithMessage("IsDeleted must be true or false if provided.");

        RuleFor(x => x)
            .Must(x => !(x.IsDeleted == true && x.IsUsed == true))
            .WithMessage(
                "A deleted category cannot be active (IsDeleted = true while IsUsed = true)."
            );
    }
}
