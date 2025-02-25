using FluentValidation;
using Application.Dtos.Reservations;

public class ReservationUpdateDtoValidator : AbstractValidator<ReservationUpdateDto>
{
    public ReservationUpdateDtoValidator()
    {
        RuleFor(x => x.DateTime)
            .GreaterThan(DateTime.UtcNow).WithMessage("Reservation date and time must be in the future.")
            .When(x => x.DateTime.HasValue);

        RuleFor(x => x.CapacityNeeded)
            .GreaterThan(0).WithMessage("Number of people must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Number of people must not exceed 20.")
            .When(x => x.CapacityNeeded.HasValue);

        RuleFor(x => x.TableId)
            .NotEmpty().WithMessage("Table ID must not be empty.")
            .When(x => x.TableId.HasValue);
    }
}