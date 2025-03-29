using Application.Dtos.Common;
using Application.Dtos.SubCategories;

namespace Application.Contracts;

public interface ISubCategoryService
{
    Task<ResultDto<SubCategoryReadDto>> CreateSubCategory(
        SubCategoryCreateDto subCategoryCreateDto
    );
    Task<ResultDto<SubCategoryReadDto>> GetSubCategory(Guid id);
    Task<ResultDto<List<SubCategoryReadDto>>> GetSubCategories();
    Task<ResultDto<SubCategoryReadDto>> UpdateSubCategory(
        Guid id,
        SubCategoryUpdateDto subCategoryUpdateDto
    );
    Task<ResultDto<bool>> DeleteSubCategory(Guid id);
}
