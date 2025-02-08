using Application.Dtos.Common;
using Application.Dtos.Ingredients;

namespace Application.Contracts;

public interface IIngredientService
{
    Task<ResultDto<IngredientReadDto>> CreateIngredient(IngredientCreateDto ingredientCreateDto);
    Task<ResultDto<IngredientReadDto>> GetIngredient(Guid id);
    Task<ResultDto<List<IngredientReadDto>>> GetAllIngredients();
    Task<ResultDto<IngredientReadDto>> UpdateIngredient(IngredientUpdateDto ingredientUpdateDto, Guid id);
    Task<ResultDto<bool>> DeleteIngredient(Guid id);
}
