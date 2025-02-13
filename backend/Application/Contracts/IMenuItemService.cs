using Application.Dtos.Common;
using Application.Dtos.MenuItems;

namespace Application.Contracts;

public interface IMenuItemService
{
    Task<ResultDto<MenuItemReadDto>> CreateMenuItem(MenuItemCreateDto menuItemCreateDto);
    Task<ResultDto<MenuItemReadDto>> GetMenuItem(Guid id);
    Task<ResultDto<List<MenuItemReadDto>>> GetMenuItemsByCategory(Guid categoryId);
    Task<ResultDto<List<MenuItemReadDto>>> GetAllMenuItems();
    Task<ResultDto<MenuItemReadDto>> UpdateMenuItem(MenuItemUpdateDto menuItemUpdateDto, Guid id);
    Task<ResultDto<bool>> DeleteMenuItem(Guid id);
}