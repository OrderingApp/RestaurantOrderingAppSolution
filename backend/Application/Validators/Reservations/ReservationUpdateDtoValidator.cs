using Application.Dtos.Reservations;
using FluentValidation;

public class ReservationUpdateDtoValidator : AbstractValidator<ReservationUpdateDto>
{
    public ReservationUpdateDtoValidator()
    {
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
    }
}
