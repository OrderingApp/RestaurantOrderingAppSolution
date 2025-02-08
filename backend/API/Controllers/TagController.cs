using Application.Contracts;
using Application.Dtos.Tags;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Manages tags used for categorizing menu items.
/// </summary>
public class TagsController(ITagService tagService) : BaseApiController
{
    /// <summary>
    /// Creates a new tag.
    /// </summary>
    /// <param name="tagCreateDto">The tag details.</param>
    /// <returns>The created tag.</returns>
    /// <response code="201">If the tag was successfully created.</response>
    /// <response code="400">If the request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateDto tagCreateDto) =>
        HandleResult(await tagService.CreateTag(tagCreateDto));

    /// <summary>
    /// Retrieves all tags.
    /// </summary>
    /// <returns>A list of tags.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetAllTags() =>
        HandleResult(await tagService.GetAllTags());

    /// <summary>
    /// Retrieves a specific tag by ID.
    /// </summary>
    /// <param name="id">The unique tag ID.</param>
    /// <returns>The requested tag.</returns>
    /// <response code="200">Returns the tag.</response>
    /// <response code="404">If the tag is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTag(Guid id) =>
        HandleResult(await tagService.GetTag(id));

    /// <summary>
    /// Updates an existing tag.
    /// </summary>
    /// <param name="id">The ID of the tag to update.</param>
    /// <param name="tagUpdateDto">The updated tag details.</param>
    /// <returns>The updated tag.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the tag is not found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateTag([FromBody] TagUpdateDto tagUpdateDto, Guid id) =>
        HandleResult(await tagService.UpdateTag(tagUpdateDto, id));

    /// <summary>
    /// Deletes a tag.
    /// </summary>
    /// <param name="id">The tag ID to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the deletion was successful.</response>
    /// <response code="404">If the tag is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteTag(Guid id) =>
        HandleResult(await tagService.DeleteTag(id));
}
