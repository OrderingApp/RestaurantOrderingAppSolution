using Application.Contracts;
using Application.Dtos.SubCategories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages sub-categories.
/// </summary>
[Route("api/sub-categories")]
public class SubCategoryController(ISubCategoryService subCategoryService) : BaseApiController
{
    /// <summary>
    /// Creates a new sub-category.
    /// </summary>
    /// <param name="subCategoryCreateDto">The sub-category data to create.</param>
    /// <returns>The created sub-category.</returns>
    /// <response code="201">Returns the newly created sub-category.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SubCategoryReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<SubCategoryReadDto>> CreateSubCategory(
        [FromBody] SubCategoryCreateDto subCategoryCreateDto) =>
        HandleResult(await subCategoryService.CreateSubCategory(subCategoryCreateDto));

    /// <summary>
    /// Retrieves a specific sub-category by ID.
    /// </summary>
    /// <param name="id">The unique sub-category ID.</param>
    /// <returns>The requested sub-category.</returns>
    /// <response code="200">Returns the sub-category.</response>
    /// <response code="404">If the sub-category is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SubCategoryReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SubCategoryReadDto>> GetSubCategory([FromRoute] Guid id) =>
        HandleResult(await subCategoryService.GetSubCategory(id));

    /// <summary>
    /// Retrieves all sub-categories.
    /// </summary>
    /// <returns>A list of sub-categories.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<SubCategoryReadDto>), 200)]
    public async Task<ActionResult<List<SubCategoryReadDto>>> GetSubCategories() =>
        HandleResult(await subCategoryService.GetSubCategories());

    /// <summary>
    /// Updates an existing sub-category.
    /// </summary>
    /// <param name="id">The ID of the sub-category to update.</param>
    /// <param name="subCategoryUpdateDto">The updated sub-category data.</param>
    /// <returns>The updated sub-category.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the sub-category is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SubCategoryReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<SubCategoryReadDto>> UpdateSubCategory(
        [FromRoute] Guid id,
        [FromBody] SubCategoryUpdateDto subCategoryUpdateDto) =>
        HandleResult(await subCategoryService.UpdateSubCategory(id, subCategoryUpdateDto));

    /// <summary>
    /// Deletes a sub-category.
    /// </summary>
    /// <param name="id">The ID of the sub-category to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the sub-category is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteSubCategory([FromRoute] Guid id) =>
        HandleResult(await subCategoryService.DeleteSubCategory(id));
}