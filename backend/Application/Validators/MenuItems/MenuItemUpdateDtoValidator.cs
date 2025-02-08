using Application.Dtos.MenuItems;
using FluentValidation;

namespace Application.Validators;

public class MenuItemUpdateDtoValidator : AbstractValidator<MenuItemUpdateDto>
{
    public MenuItemUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null).WithMessage("Name cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().When(x => x.Description != null).WithMessage("Description cannot be empty if provided.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleForEach(x => x.TagIds)
            .NotEmpty().WithMessage("Tag ID cannot be empty.");
    }
}
