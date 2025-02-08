using Application.Contracts;
using Application.Dtos.Tags;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class TagsController(ITagService tagService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] TagCreateDto tagCreateDto) =>
        HandleResult(await tagService.CreateTag(tagCreateDto));

    [HttpGet]
    public async Task<IActionResult> GetAllTags() =>
        HandleResult(await tagService.GetAllTags());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTag(Guid id) =>
        HandleResult(await tagService.GetTag(id));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag([FromBody] TagUpdateDto tagUpdateDto, Guid id) =>
        HandleResult(await tagService.UpdateTag(tagUpdateDto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(Guid id) =>
        HandleResult(await tagService.DeleteTag(id));
}