using Application.Contracts;
using Application.Dtos.MenuCategories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages menu categories.
/// </summary>
public class MenuCategoryController(IMenuCategoryService menuCategoryService) : BaseApiController
{
    /// <summary>
    /// Creates a new menu category.
    /// </summary>
    /// <param name="menuCategoryCreateDto">The menu category data to create.</param>
    /// <returns>The created menu category.</returns>
    /// <response code="201">Returns the newly created menu category.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateMenuCategory([FromBody] MenuCategoryCreateDto menuCategoryCreateDto) =>
        HandleResult(await menuCategoryService.CreateMenuCategory(menuCategoryCreateDto));

    /// <summary>
    /// Retrieves a specific menu category by ID.
    /// </summary>
    /// <param name="id">The unique menu category ID.</param>
    /// <returns>The requested menu category.</returns>
    /// <response code="200">Returns the menu category.</response>
    /// <response code="404">If the menu category is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMenuCategory(Guid id) =>
        HandleResult(await menuCategoryService.GetMenuCategory(id));

    /// <summary>
    /// Retrieves all menu categories.
    /// </summary>
    /// <returns>A list of menu categories.</returns>
    [HttpGet("menu-categories")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllMenuCategories() =>
        HandleResult(await menuCategoryService.GetAllMenuCategories());

    /// <summary>
    /// Updates an existing menu category.
    /// </summary>
    /// <param name="id">The ID of the menu category to update.</param>
    /// <param name="menuCategoryUpdateDto">The updated menu category data.</param>
    /// <returns>The updated menu category.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the menu category is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMenuCategory([FromBody] MenuCategoryUpdateDto menuCategoryUpdateDto, Guid id) =>
        HandleResult(await menuCategoryService.UpdateMenuCategory(menuCategoryUpdateDto, id));

    /// <summary>
    /// Deletes a menu category.
    /// </summary>
    /// <param name="id">The ID of the menu category to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the menu category is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteMenuCategory(Guid id) =>
        HandleResult(await menuCategoryService.DeleteMenuCategory(id));
}
