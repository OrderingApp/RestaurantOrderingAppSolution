using Application.Contracts;
using Application.Dtos.Areas;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages restaurant areas, including retrieval, creation, updates, and deletion.
/// </summary>
[Route("api/areas")]
public class AreaController(IAreaService areaService) : BaseApiController
{

    /// <summary>
    /// Creates a new area.
    /// </summary>
    /// <param name="areaCreateDto">The area data.</param>
    /// <returns>The created area.</returns>
    /// <response code="201">If the area is successfully created.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AreaReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AreaReadDto>> CreateArea([FromBody] AreaCreateDto areaCreateDto) =>
        HandleResult(await areaService.CreateArea(areaCreateDto));

    /// <summary>
    /// Retrieves an area by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the area.</param>
    /// <returns>The area details.</returns>
    /// <response code="200">Returns the area.</response>
    /// <response code="404">If the area is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AreaReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AreaReadDto>> GetArea([FromRoute] Guid id) =>
        HandleResult(await areaService.GetArea(id));

    /// <summary>
    /// Retrieves all areas.
    /// </summary>
    /// <returns>List of areas.</returns>
    /// <response code="200">Returns the list of areas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<AreaReadDto>), 200)]
    public async Task<ActionResult<List<AreaReadDto>>> GetAreas() =>
        HandleResult(await areaService.GetAreas());

    /// <summary>
    /// Updates an existing area.
    /// </summary>
    /// <param name="id">The unique identifier of the area.</param>
    /// <param name="areaUpdateDto">The updated area details.</param>
    /// <returns>The updated area.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the area is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AreaReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AreaReadDto>> UpdateArea([FromRoute] Guid id, [FromBody] AreaUpdateDto areaUpdateDto) =>
        HandleResult(await areaService.UpdateArea(id, areaUpdateDto));

    /// <summary>
    /// Deletes an area by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the area.</param>
    /// <returns>True if the deletion was successful.</returns>
    /// <response code="200">If the deletion was successful.</response>
    /// <response code="404">If the area is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteArea([FromRoute] Guid id) =>
        HandleResult(await areaService.DeleteArea(id));
}
