using Application.Dtos.Tables;
using FluentValidation;

public class TableUpdateDtoValidator : AbstractValidator<TableUpdateDto>
{
    public TableUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50).WithMessage("Table name must not exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("Capacity must not exceed 20.")
            .When(x => x.Capacity.HasValue);

        RuleFor(x => x.IsUsed)
            .NotNull().WithMessage("IsUsed flag must be specified.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid table status specified.")
            .When(x => x.Status.HasValue);
    }
}