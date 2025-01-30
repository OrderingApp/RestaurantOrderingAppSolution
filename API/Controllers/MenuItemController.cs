using Application.Contracts;
using Application.Dtos.MenuItems;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MenuItemController(IMenuItemService menuItemService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemCreateDto menuItemCreateDto) =>
        HandleResult(await menuItemService.CreateMenuItem(menuItemCreateDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuItem(Guid id) =>
        HandleResult(await menuItemService.GetMenuItem(id));

    [HttpGet]
    public async Task<IActionResult> GetAllMenuItems() =>
        HandleResult(await menuItemService.GetAllMenuItems());

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetMenuItemsByCategory(Guid categoryId, [FromQuery] Guid? tagId) =>
        HandleResult(await menuItemService.GetMenuItemsByCategory(categoryId, tagId));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenuItem([FromBody] MenuItemUpdateDto menuItemUpdateDto, Guid id) =>
        HandleResult(await menuItemService.UpdateMenuItem(menuItemUpdateDto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuItem(Guid id) =>
        HandleResult(await menuItemService.DeleteMenuItem(id));
}
