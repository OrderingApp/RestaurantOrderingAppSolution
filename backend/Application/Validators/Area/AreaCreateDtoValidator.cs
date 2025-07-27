using Application.Dtos.Areas;
using FluentValidation;

namespace Application.Validators.Area;

public class AreaCreateDtoValidator : AbstractValidator<AreaCreateDto>
{
    public AreaCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Area name is required.");
    }
}
