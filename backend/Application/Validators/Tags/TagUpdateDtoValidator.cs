using FluentValidation;
using Application.Dtos.Tags;

public class TagUpdateDtoValidator : AbstractValidator<TagUpdateDto>
{
    public TagUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required.")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.");

        RuleFor(x => x.IsUsed)
            .NotNull().WithMessage("IsUsed flag must be specified.");
    }
}
