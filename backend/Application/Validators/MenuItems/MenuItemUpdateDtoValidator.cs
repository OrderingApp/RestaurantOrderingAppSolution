using Application.Dtos.MenuItems;
using FluentValidation;

namespace Application.Validators;

public class MenuItemUpdateDtoValidator : AbstractValidator<MenuItemUpdateDto>
{
    public MenuItemUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().When(x => x.Name != null)
            .WithMessage("Name cannot be empty if provided.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().When(x => x.Description != null)
            .WithMessage("Description cannot be empty if provided.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).When(x => x.Price.HasValue)
            .WithMessage("Price must be a non-negative value.");

        RuleFor(x => x.IngredientIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Ingredient IDs must be unique.");
    }
}