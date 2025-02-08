using Application.Dtos.Tables;
using FluentValidation;

namespace Application.Validators.Tables;

public class TableCreateDtoValidator : AbstractValidator<TableCreateDto>
{
    public TableCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Table name is required.")
            .MaximumLength(50).WithMessage("Table name must not exceed 50 characters.");

        RuleFor(x => x.NumberOfPeople)
            .GreaterThan(0).WithMessage("Number of people must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("Number of people must not exceed 20.");
    }
}
