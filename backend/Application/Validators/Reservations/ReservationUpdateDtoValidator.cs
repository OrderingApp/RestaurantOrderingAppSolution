using FluentValidation;
using Application.Dtos.Reservations;

public class ReservationUpdateDtoValidator : AbstractValidator<ReservationUpdateDto>
{
    public ReservationUpdateDtoValidator()
    {
        RuleFor(x => x.ReservationDateTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Reservation date and time must be in the future.")
            .When(x => x.ReservationDateTime.HasValue);

        RuleFor(x => x.NumberOfPeople)
            .GreaterThan(0).WithMessage("Number of people must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Number of people must not exceed 20.")
            .When(x => x.NumberOfPeople.HasValue);

        RuleFor(x => x.TableId)
            .NotEmpty().WithMessage("Table ID must not be empty.")
            .When(x => x.TableId.HasValue);
    }
}