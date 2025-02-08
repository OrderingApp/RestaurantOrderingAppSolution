using FluentValidation;
using Application.Dtos.Reservations;

public class ReservationCreateDtoValidator : AbstractValidator<ReservationCreateDto>
{
    public ReservationCreateDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d{9,15}$").WithMessage("Phone number must contain between 9 and 15 digits.");

        RuleFor(x => x.Surname)
            .NotEmpty().WithMessage("Surname is required.")
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters.");

        RuleFor(x => x.ReservationDateTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Reservation date and time must be in the future.");

        RuleFor(x => x.NumberOfPeople)
            .GreaterThan(0).WithMessage("Number of people must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Number of people must not exceed 20.");
    }
}
