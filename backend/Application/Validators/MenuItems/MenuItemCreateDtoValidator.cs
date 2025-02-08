using Application.Dtos.MenuItems;
using FluentValidation;

namespace Application.Validators;

public class MenuItemCreateDtoValidator : AbstractValidator<MenuItemCreateDto>
{
    public MenuItemCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.MenuCategoryId)
            .NotEmpty().WithMessage("MenuCategoryId is required.");

        RuleForEach(x => x.TagIds)
            .NotEmpty().WithMessage("Tag ID cannot be empty.");
    }
}