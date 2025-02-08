using Application.Dtos.MenuCategories;
using FluentValidation;

namespace Application.Validators;

public class MenuCategoryUpdateDtoValidator : AbstractValidator<MenuCategoryUpdateDto>
{
    public MenuCategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null).WithMessage("Name cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.IsUsed)
            .NotNull().WithMessage("IsUsed flag is required.");

        RuleFor(x => x.IsDeleted)
            .NotNull().WithMessage("IsDeleted flag is required.");
    }
}
