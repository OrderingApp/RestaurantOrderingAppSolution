using Application.Dtos.Common;
using Application.Dtos.Ingredients;

namespace Application.Contracts;

public interface IIngredientService
{
    Task<ResultDto<IngredientReadDto>> CreateIngredient(IngredientCreateDto ingredientCreateDto);
    Task<ResultDto<IngredientReadDto>> GetIngredient(Guid id);
    Task<ResultDto<List<IngredientReadDto>>> GetIngredients(List<string>? tags = null);
    Task<ResultDto<IngredientReadDto>> AddTagsToIngredient(Guid id, List<Guid> tagIds);
    Task<ResultDto<IngredientReadDto>> UpdateIngredient(
        Guid id,
        IngredientUpdateDto ingredientUpdateDto
    );
    Task<ResultDto<bool>> DeleteIngredient(Guid id);
}
