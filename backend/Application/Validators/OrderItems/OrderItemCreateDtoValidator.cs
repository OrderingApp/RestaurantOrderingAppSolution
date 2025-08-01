using Application.Dtos.OrderItemIngredients;
using Application.Dtos.OrderItems;
using FluentValidation;

namespace Application.Validators;

public class OrderItemCreateDtoValidator : AbstractValidator<OrderItemCreateDto>
{
    public OrderItemCreateDtoValidator()
    {
        RuleFor(x => x.MenuItemId).NotEmpty().WithMessage("MenuItemId is required.");

        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(200)
            .WithMessage("Special instructions must not exceed 200 characters.");

        RuleForEach(x => x.ExtraIngredients ?? new List<OrderItemIngredientAddDto>())
            .ChildRules(extra =>
            {
                extra.RuleFor(i => i.IngredientId)
                     .NotEmpty()
                     .WithMessage("IngredientId is required.");

                extra.RuleFor(i => i.Quantity)
                     .GreaterThan(0)
                     .WithMessage("Ingredient quantity must be greater than zero.")
                     .LessThanOrEqualTo(2)
                     .WithMessage("Ingredient quantity cannot exceed 2.");
            });

        RuleForEach(x => x.RemovedIngredientIds ?? new List<Guid>())
            .NotEmpty()
            .WithMessage("RemovedIngredientId must not be empty.");

        RuleFor(x => x)
            .Must(x =>
            {
                var extras = x.ExtraIngredients?.Select(e => e.IngredientId) ?? Enumerable.Empty<Guid>();
                var removed = x.RemovedIngredientIds ?? Enumerable.Empty<Guid>();
                return !extras.Intersect(removed).Any();
            })
            .WithMessage("An ingredient cannot be both added as an extra and removed.");
    }
}
