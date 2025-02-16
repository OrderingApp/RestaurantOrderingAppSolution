using Application.Dtos.OrderItems;
using FluentValidation;

namespace Application.Validators;

public class OrderItemUpdateDtoValidator : AbstractValidator<OrderItemUpdateDto>
{
    public OrderItemUpdateDtoValidator()
    {
        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(500).WithMessage("Special instructions must not exceed 500 characters.");

        RuleFor(x => x.Discount)
            .InclusiveBetween(0, 100).WithMessage("Discount must be between 0% and 100%.")
            .When(x => x.Discount.HasValue);

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
