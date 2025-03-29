using Application.Contracts;
using Application.Dtos.Common;
using Application.Dtos.MenuCategories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages menu categories.
/// </summary>
[Route("api/menu-categories")]
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
    [ProducesResponseType(typeof(MenuCategoryReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<MenuCategoryReadDto>> CreateMenuCategory(
        [FromBody] MenuCategoryCreateDto menuCategoryCreateDto
    ) => HandleResult(await menuCategoryService.CreateMenuCategory(menuCategoryCreateDto));

    /// <summary>
    /// Retrieves a specific menu category by ID.
    /// </summary>
    /// <param name="id">The unique menu category ID.</param>
    /// <returns>The requested menu category.</returns>
    /// <response code="200">Returns the menu category.</response>
    /// <response code="404">If the menu category is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MenuCategoryReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MenuCategoryReadDto>> GetMenuCategory([FromRoute] Guid id) =>
        HandleResult(await menuCategoryService.GetMenuCategory(id));

    /// <summary>
    /// Retrieves all menu categories.
    /// </summary>
    /// <returns>A list of menu categories.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<MenuCategoryReadDto>), 200)]
    public async Task<ActionResult<List<MenuCategoryReadDto>>> GetMenuCategories() =>
        HandleResult(await menuCategoryService.GetMenuCategories());

    /// <summary>
    /// Retrieves all menu categories, including their subcategories and associated menu items.
    /// Supports optional filtering by category, subcategory, and tags.
    /// </summary>
    /// <param name="request">The request containing optional filters and pagination parameters.</param>
    /// <returns>A paginated list of menu categories with their hierarchical structure.</returns>
    /// <response code="200">Returns the list of menu categories with hierarchy.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("hierarchy")]
    [ProducesResponseType(typeof(List<MenuCategoryHierarchyReadDto>), 200)]
    public async Task<
        ActionResult<List<MenuCategoryHierarchyReadDto>>
    > GetMenuCategoriesWithHierarchy([FromQuery] GetMenuCategoryHierarchyRequest request) =>
        HandleResult(await menuCategoryService.GetMenuCategoriesWithHierarchy(request));

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
    [ProducesResponseType(typeof(MenuCategoryReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<MenuCategoryReadDto>> UpdateMenuCategory(
        [FromRoute] Guid id,
        [FromBody] MenuCategoryUpdateDto menuCategoryUpdateDto
    ) => HandleResult(await menuCategoryService.UpdateMenuCategory(id, menuCategoryUpdateDto));

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
    public async Task<IActionResult> DeleteMenuCategory([FromRoute] Guid id) =>
        HandleResult(await menuCategoryService.DeleteMenuCategory(id));
}
