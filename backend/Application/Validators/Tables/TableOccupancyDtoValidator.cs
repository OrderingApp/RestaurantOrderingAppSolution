using FluentValidation;
using Application.Dtos.Tables;

public class TableOccupancyDtoValidator : AbstractValidator<TableOccupancyDto>
{
    public TableOccupancyDtoValidator()
    {
        RuleFor(x => x.IsOccupied)
            .NotNull().WithMessage("IsOccupied flag must be specified.");
    }
}
