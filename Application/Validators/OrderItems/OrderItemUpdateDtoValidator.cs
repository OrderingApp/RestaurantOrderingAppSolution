using Application.Dtos.OrderItems;
using FluentValidation;

namespace Application.Validators;

public class OrderItemUpdateDtoValidator : AbstractValidator<OrderItemUpdateDto>
{
    public OrderItemUpdateDtoValidator()
    {
        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(500).WithMessage("Special instructions must not exceed 500 characters.");

        RuleForEach(x => x.Ingredients)
            .ChildRules(ingredient =>
            {
                ingredient.RuleFor(i => i.IngredientId)
                    .NotEmpty().WithMessage("IngredientId is required.");
                ingredient.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Ingredient quantity must be greater than zero.");
            });
    }
}
