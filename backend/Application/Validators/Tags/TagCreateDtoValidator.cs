using FluentValidation;
using Application.Dtos.Tags;

public class TagCreateDtoValidator : AbstractValidator<TagCreateDto>
{
    public TagCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required.")
            .MaximumLength(50).WithMessage("Tag name must not exceed 50 characters.");
    }
}
