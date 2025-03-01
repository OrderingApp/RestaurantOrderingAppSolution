using Application.Dtos.Common;
using Application.Dtos.MenuItems;

namespace Application.Contracts;

public interface IMenuItemService
{
    Task<ResultDto<MenuItemReadDto>> CreateMenuItem(MenuItemCreateDto menuItemCreateDto);
    Task<ResultDto<MenuItemReadDto>> GetMenuItem(Guid id);
    Task<ResultDto<List<MenuItemReadDto>>> GetMenuItems(Guid? categoryId = null, List<Guid>? ingredientIds = null, List<string>? tags = null);
    Task<ResultDto<MenuItemReadDto>> UpdateMenuItem(Guid id, MenuItemUpdateDto menuItemUpdateDto);
    Task<ResultDto<bool>> DeleteMenuItem(Guid id);
}