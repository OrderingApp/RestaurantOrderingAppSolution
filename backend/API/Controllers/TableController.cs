using Application.Contracts;
using Application.Dtos.Tables;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages tables in the restaurant.
/// </summary>
[Route("api/tables")]
public class TableController(ITableService tableService) : BaseApiController
{
    /// <summary>
    /// Creates a new table.
    /// </summary>
    /// <param name="tableCreateDto">The table details.</param>
    /// <returns>The created table.</returns>
    /// <response code="201">If the table was successfully created.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateTable([FromBody] TableCreateDto tableCreateDto) =>
        HandleResult(await tableService.CreateTable(tableCreateDto));

    /// <summary>
    /// Retrieves a specific table by ID.
    /// </summary>
    /// <param name="id">The unique table ID.</param>
    /// <returns>The requested table.</returns>
    /// <response code="200">Returns the table.</response>
    /// <response code="404">If the table is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTable(Guid id) =>
        HandleResult(await tableService.GetTable(id));

    /// <summary>
    /// Retrieves all tables.
    /// </summary>
    /// <returns>A list of tables.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetTables() => HandleResult(await tableService.GetTables());

    /// <summary>
    /// Updates an existing table.
    /// </summary>
    /// <param name="id">The ID of the table to update.</param>
    /// <param name="tableUpdateDto">The updated table details.</param>
    /// <returns>The updated table.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the table is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTable(
        Guid id,
        [FromBody] TableUpdateDto tableUpdateDto
    ) => HandleResult(await tableService.UpdateTable(id, tableUpdateDto));

    /// <summary>
    /// Updates the status of a table.
    /// </summary>
    /// <param name="id">The ID of the table to update.</param>
    /// <param name="status">The updated status details.</param>
    /// <returns>The updated table status.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the table is not found.</response>
    [HttpPut("{id}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TableStatus status) =>
        HandleResult(await tableService.UpdateStatus(id, status));

    /// <summary>
    /// Deletes a table.
    /// </summary>
    /// <param name="id">The table ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the table is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTable(Guid id) =>
        HandleResult(await tableService.DeleteTable(id));
}
