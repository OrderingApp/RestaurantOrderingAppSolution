using FluentValidation;
using Application.Dtos.Tables;

public class TableUpdateDtoValidator : AbstractValidator<TableUpdateDto>
{
    public TableUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Table name must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.NumberOfPeople)
            .GreaterThan(0).WithMessage("The number of people must be greater than 0.")
            .LessThanOrEqualTo(20).WithMessage("The number of people must not exceed 20.");

        RuleFor(x => x.IsUsed)
            .NotNull().WithMessage("IsUsed flag must be specified.");
    }
}