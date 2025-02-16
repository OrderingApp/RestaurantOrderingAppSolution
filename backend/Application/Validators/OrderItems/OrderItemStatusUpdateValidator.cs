using Domain;
using FluentValidation;

namespace Application.Validators;

public class OrderItemStatusUpdateValidator : AbstractValidator<OrderItemStatus>
{
    public OrderItemStatusUpdateValidator()
    {
        RuleFor(x => x)
            .IsInEnum().WithMessage("Invalid status value. Allowed values: Pending, Served, Cancelled.");
    }
}