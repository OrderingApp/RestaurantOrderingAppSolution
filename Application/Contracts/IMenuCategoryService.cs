using Application.Dtos.Common;
using Application.Dtos.MenuCategories;

namespace Application.Contracts;

public interface IMenuCategoryService
{
    Task<ResultDto<MenuCategoryReadDto>> CreateMenuCategory(MenuCategoryCreateDto menuCategoryCreateDto);
    Task<ResultDto<MenuCategoryReadDto>> GetMenuCategory(Guid id);
    Task<ResultDto<List<MenuCategoryReadDto>>> GetAllMenuCategories();
    Task<ResultDto<MenuCategoryReadDto>> UpdateMenuCategory(MenuCategoryUpdateDto menuCategoryUpdateDto, Guid id);
    Task<ResultDto<bool>> DeleteMenuCategory(Guid id);
}