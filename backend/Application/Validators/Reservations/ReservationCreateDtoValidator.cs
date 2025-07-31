using Application.Dtos.Reservations;
using FluentValidation;

public class ReservationCreateDtoValidator : AbstractValidator<ReservationCreateDto>
{
    public ReservationCreateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(@"^\d{9,15}$")
            .WithMessage("Phone number must contain between 9 and 15 digits.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.ScheduledFor)
            .NotNull()
            .WithMessage("Reservation date and time is required.")
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Reservation date and time must be in the future.");

        RuleFor(x => x.CapacityNeeded)
            .GreaterThan(0)
            .WithMessage("Number of people must be greater than 0.")
            .LessThanOrEqualTo(40)
            .WithMessage("Number of people must not exceed 40.");
    }
}
