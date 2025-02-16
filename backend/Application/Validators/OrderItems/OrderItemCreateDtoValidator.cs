using Application.Dtos.OrderItems;
using FluentValidation;

namespace Application.Validators;

public class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto>
{
    public OrderItemCreateDtoValidator()
    {
        RuleFor(x => x.MenuItemId)
            .NotEmpty().WithMessage("MenuItemId is required.");

        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(200).WithMessage("Special instructions must not exceed 200 characters.");

        RuleForEach(x => x.ExtraIngredients)
            .ChildRules(extra =>
            {
                extra.RuleFor(i => i.IngredientId)
                    .NotEmpty().WithMessage("IngredientId is required.");

                extra.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Ingredient quantity must be greater than zero.");
            });

        RuleForEach(x => x.RemovedIngredients)
            .ChildRules(removed =>
            {
                removed.RuleFor(i => i.IngredientId)
                    .NotEmpty().WithMessage("IngredientId is required.");
            });
    }
}
