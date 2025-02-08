using Application.Dtos.Orders;
using FluentValidation;

public class OrderUpdateTypeDtoValidator : AbstractValidator<OrderUpdateTypeDto>
{
    public OrderUpdateTypeDtoValidator()
    {
        RuleFor(x => x.TableId)
            .NotEmpty().When(x => x.TableId.HasValue).WithMessage("TableId must be a valid GUID.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .Matches(@"^\d{9,15}$").WithMessage("PhoneNumber must be between 9 and 15 digits.");

        RuleFor(x => x.Address)
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("Address cannot be empty if provided.");

        RuleFor(x => x.AdditionalInstructions)
            .MaximumLength(500).WithMessage("AdditionalInstructions must not exceed 500 characters.");
    }
}
