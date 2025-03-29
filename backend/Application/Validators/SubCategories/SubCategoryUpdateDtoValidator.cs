using Application.Dtos.SubCategories;
using FluentValidation;

namespace Application.Validators.SubCategories;

public class SubCategoryUpdateDtoValidator : AbstractValidator<SubCategoryUpdateDto>
{
    public SubCategoryUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.MenuCategoryId).NotEmpty().WithMessage("MenuCategoryId is required.");
    }
}
