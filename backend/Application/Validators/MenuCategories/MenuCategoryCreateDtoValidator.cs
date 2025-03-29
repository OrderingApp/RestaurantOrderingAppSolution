using Application.Dtos.MenuCategories;
using FluentValidation;

namespace Application.Validators.MenuCategories;

public class MenuCategoryCreateDtoValidator : AbstractValidator<MenuCategoryCreateDto>
{
    public MenuCategoryCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MinimumLength(2)
            .WithMessage("Name must be at least 2 characters.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name cannot be empty or only spaces.");
    }
}
