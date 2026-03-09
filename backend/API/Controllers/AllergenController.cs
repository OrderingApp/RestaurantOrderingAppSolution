using Application.Contracts;
using Application.Dtos.Allergens;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages allergens used for categorizing ingredients.
/// </summary>
[Route("api/allergens")]
public class AllergenController(IAllergenService allergenService) : BaseApiController
{
    /// <summary>
    /// Creates a new allergen.
    /// </summary>
    /// <param name="allergenCreateDto">The allergen details.</param>
    /// <returns>The created allergen.</returns>
    /// <response code="201">If the allergen was successfully created.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateAllergen([FromBody] AllergenCreateDto allergenCreateDto) =>
        HandleResult(await allergenService.CreateAllergen(allergenCreateDto));

    /// <summary>
    /// Retrieves all allergens.
    /// </summary>
    /// <returns>A list of allergens.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllAllergens() =>
        HandleResult(await allergenService.GetAllAllergens());

    /// <summary>
    /// Retrieves a specific allergen by ID.
    /// </summary>
    /// <param name="id">The unique allergen ID.</param>
    /// <returns>The requested allergen.</returns>
    /// <response code="200">Returns the allergen.</response>
    /// <response code="404">If the allergen is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetAllergen(Guid id) =>
        HandleResult(await allergenService.GetAllergen(id));

    /// <summary>
    /// Updates an existing allergen.
    /// </summary>
    /// <param name="id">The ID of the allergen to update.</param>
    /// <param name="allergenUpdateDto">The updated allergen details.</param>
    /// <returns>The updated allergen.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the allergen is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateAllergen(
        [FromBody] AllergenUpdateDto allergenUpdateDto,
        Guid id
    ) => HandleResult(await allergenService.UpdateAllergen(allergenUpdateDto, id));

    /// <summary>
    /// Deletes an allergen.
    /// </summary>
    /// <param name="id">The allergen ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the allergen is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAllergen(Guid id) =>
        HandleResult(await allergenService.DeleteAllergen(id));
}
