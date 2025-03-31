using Application.Dtos.MenuItems;
using FluentValidation;

namespace Application.Validators;

public class MenuItemCreateDtoValidator : AbstractValidator<MenuItemCreateDto>
{
    public MenuItemCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Menu item name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be a non-negative value.");

        RuleFor(x => x.MenuCategoryId).NotEmpty().WithMessage("Menu category ID is required.");

        RuleFor(x => x.IngredientIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Ingredient IDs must be unique.");
    }
}
