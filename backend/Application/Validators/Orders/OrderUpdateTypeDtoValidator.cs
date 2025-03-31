using Application.Dtos.Orders;
using Domain;
using FluentValidation;

public class OrderUpdateTypeDtoValidator : AbstractValidator<OrderUpdateTypeDto>
{
    public OrderUpdateTypeDtoValidator()
    {
        RuleFor(x => x.TableId)
            .NotEmpty()
            .WithMessage("TableId must be a valid GUID when provided.")
            .When(x => x.NewOrderType == OrderType.DineIn);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("PhoneNumber is required for Takeaway and Delivery orders.")
            .Matches(@"^\d{9,15}$")
            .WithMessage("PhoneNumber must be between 9 and 15 digits.")
            .When(x =>
                x.NewOrderType == OrderType.Takeaway || x.NewOrderType == OrderType.Delivery
            );

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required for Delivery orders.")
            .When(x => x.NewOrderType == OrderType.Delivery)
            .MaximumLength(255)
            .WithMessage("Address must not exceed 255 characters.");

        RuleFor(x => x.AdditionalInstructions)
            .MaximumLength(500)
            .WithMessage("AdditionalInstructions must not exceed 500 characters.");

        RuleFor(x => x.NewOrderType).IsInEnum().WithMessage("Invalid order type specified.");
    }
}
