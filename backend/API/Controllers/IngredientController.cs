using Application.Contracts;
using Application.Dtos.Ingredients;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages ingredients in the system.
/// </summary>
[Route("api/ingredients")]
public class IngredientsController(IIngredientService ingredientService) : BaseApiController
{
    /// <summary>
    /// Creates a new ingredient.
    /// </summary>
    /// <param name="ingredientCreateDto">The ingredient data to create.</param>
    /// <returns>The created ingredient.</returns>
    /// <response code="201">Returns the newly created ingredient.</response>
    /// <response code="400">If the input is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(IngredientReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IngredientReadDto>> CreateIngredient(
        [FromBody] IngredientCreateDto ingredientCreateDto
    ) => HandleResult(await ingredientService.CreateIngredient(ingredientCreateDto));

    /// <summary>
    /// Retrieves a specific ingredient by ID.
    /// </summary>
    /// <param name="id">The unique ingredient ID.</param>
    /// <returns>The requested ingredient.</returns>
    /// <response code="200">Returns the ingredient.</response>
    /// <response code="404">If the ingredient is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IngredientReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IngredientReadDto>> GetIngredient([FromRoute] Guid id) =>
        HandleResult(await ingredientService.GetIngredient(id));

    /// <summary>
    /// Retrieves all ingredients.
    /// </summary>
    /// <returns>A list of all available ingredients.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<IngredientReadDto>), 200)]
    public async Task<ActionResult<List<IngredientReadDto>>> GetIngredients(
        [FromQuery] List<string>? tags = null
    ) => HandleResult(await ingredientService.GetIngredients(tags));

    /// <summary>
    /// Adds tags to an ingredient.
    /// </summary>
    /// <param name="id">The unique ingredient ID.</param>
    /// <param name="tagIds">List of tag IDs to associate with the ingredient.</param>
    /// <returns>The updated ingredient with assigned tags.</returns>
    /// <response code="200">If tags were successfully added.</response>
    /// <response code="404">If the ingredient is not found.</response>
    [HttpPut("{id}/tags")]
    [ProducesResponseType(typeof(IngredientReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IngredientReadDto>> AddTagsToIngredient(
        [FromRoute] Guid id,
        [FromBody] List<Guid> tagIds
    ) => HandleResult(await ingredientService.AddTagsToIngredient(id, tagIds));

    /// <summary>
    /// Updates an ingredient.
    /// </summary>
    /// <param name="id">The ID of the ingredient to update.</param>
    /// <param name="ingredientUpdateDto">The updated ingredient data.</param>
    /// <returns>The updated ingredient.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the ingredient is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(IngredientReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IngredientReadDto>> UpdateIngredient(
        [FromRoute] Guid id,
        [FromBody] IngredientUpdateDto ingredientUpdateDto
    ) => HandleResult(await ingredientService.UpdateIngredient(id, ingredientUpdateDto));

    /// <summary>
    /// Deletes an ingredient.
    /// </summary>
    /// <param name="id">The ID of the ingredient to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the ingredient is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteIngredient([FromRoute] Guid id) =>
        HandleResult(await ingredientService.DeleteIngredient(id));
}
