using Application.Contracts;
using Application.Dtos.MenuCategories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MenuCategoryController(IMenuCategoryService menuCategoryService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateMenuCategory([FromBody] MenuCategoryCreateDto menuCategoryCreateDto) =>
        HandleResult(await menuCategoryService.CreateMenuCategory(menuCategoryCreateDto));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuCategory(Guid id) =>
        HandleResult(await menuCategoryService.GetMenuCategory(id));

    [HttpGet("menu-categories")]
    public async Task<IActionResult> GetAllMenuCategories() =>
        HandleResult(await menuCategoryService.GetAllMenuCategories());

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMenuCategory([FromBody] MenuCategoryUpdateDto menuCategoryUpdateDto, Guid id) =>
        HandleResult(await menuCategoryService.UpdateMenuCategory(menuCategoryUpdateDto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMenuCategory(Guid id) =>
        HandleResult(await menuCategoryService.DeleteMenuCategory(id));
}
