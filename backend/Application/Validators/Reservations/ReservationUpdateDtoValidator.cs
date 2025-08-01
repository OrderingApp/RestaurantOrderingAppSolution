using Application.Dtos.Reservations;
using FluentValidation;

public class ReservationUpdateDtoValidator : AbstractValidator<ReservationUpdateDto>
{
    public ReservationUpdateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9\s\-]{7,15}$")
            .WithMessage("Phone number must be valid.")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Name)
            .MinimumLength(2)
            .MaximumLength(100)
            .WithMessage("Name must be between 2 and 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));

        RuleFor(x => x.ScheduledFor)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Reservation date and time must be in the future.")
            .When(x => x.ScheduledFor.HasValue);

        RuleFor(x => x.CapacityNeeded)
            .GreaterThan(0)
            .WithMessage("Number of people must be greater than 0.")
            .LessThanOrEqualTo(20)
            .WithMessage("Number of people must not exceed 20.")
            .When(x => x.CapacityNeeded.HasValue);

        RuleFor(x => x.TableId)
            .NotEqual(Guid.Empty)
            .WithMessage("TableId must be a valid GUID.")
            .When(x => x.TableId.HasValue);
    }
}
