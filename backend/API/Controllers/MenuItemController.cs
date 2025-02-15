using Application.Contracts;
using Application.Dtos.MenuItems;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages menu items.
/// </summary>
public class MenuItemController(IMenuItemService menuItemService) : BaseApiController
{
    /// <summary>
    /// Creates a new menu item.
    /// </summary>
    /// <param name="menuItemCreateDto">The menu item data to create.</param>
    /// <returns>The created menu item.</returns>
    /// <response code="201">Returns the newly created menu item.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemCreateDto menuItemCreateDto) =>
        HandleResult(await menuItemService.CreateMenuItem(menuItemCreateDto));

    /// <summary>
    /// Retrieves a specific menu item by ID.
    /// </summary>
    /// <param name="id">The unique menu item ID.</param>
    /// <returns>The requested menu item.</returns>
    /// <response code="200">Returns the menu item.</response>
    /// <response code="404">If the menu item is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMenuItem(Guid id) =>
        HandleResult(await menuItemService.GetMenuItem(id));

    /// <summary>
    /// Retrieves all menu items.
    /// </summary>
    /// <returns>A list of menu items.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllMenuItems() =>
        HandleResult(await menuItemService.GetAllMenuItems());

    /// <summary>
    /// Retrieves menu items by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <returns>A list of menu items in the specified category.</returns>
    /// <response code="200">Returns menu items in the category.</response>
    [HttpGet("category/{categoryId}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetMenuItemsByCategory(Guid categoryId) =>
        HandleResult(await menuItemService.GetMenuItemsByCategory(categoryId));

    /// <summary>
    /// Updates an existing menu item.
    /// </summary>
    /// <param name="id">The ID of the menu item to update.</param>
    /// <param name="menuItemUpdateDto">The updated menu item data.</param>
    /// <returns>The updated menu item.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the menu item is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMenuItem([FromBody] MenuItemUpdateDto menuItemUpdateDto, Guid id) =>
        HandleResult(await menuItemService.UpdateMenuItem(menuItemUpdateDto, id));

    /// <summary>
    /// Deletes a menu item.
    /// </summary>
    /// <param name="id">The ID of the menu item to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the menu item is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteMenuItem(Guid id) =>
        HandleResult(await menuItemService.DeleteMenuItem(id));
}
