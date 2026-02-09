using Application.Dtos.Common;
using Application.Dtos.IngredientCategories;

namespace Application.Contracts;

public interface IIngredientCategoryService
{
    Task<ResultDto<IngredientCategoryReadDto>> CreateIngredientCategory(IngredientCategoryCreateDto dto);
    Task<ResultDto<List<IngredientCategoryReadDto>>> GetAllIngredientCategories();
    Task<ResultDto<IngredientCategoryReadDto>> GetIngredientCategory(Guid id);
    Task<ResultDto<IngredientCategoryReadDto>> UpdateIngredientCategory(IngredientCategoryUpdateDto dto, Guid id);
    Task<ResultDto<bool>> DeleteIngredientCategory(Guid id);
}
