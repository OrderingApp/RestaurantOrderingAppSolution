using Application.Contracts;
using Application.Dtos.IngredientCategories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/ingredient-categories")]
public class IngredientCategoriesController(IIngredientCategoryService ingredientCategoryService) : BaseApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(IngredientCategoryReadDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IngredientCategoryReadDto>> Create([FromBody] IngredientCategoryCreateDto dto) => HandleResult(await ingredientCategoryService.CreateIngredientCategory(dto));

    [HttpGet]
    [ProducesResponseType(typeof(List<IngredientCategoryReadDto>), 200)]
    public async Task<ActionResult<List<IngredientCategoryReadDto>>> GetAll() => HandleResult(await ingredientCategoryService.GetAllIngredientCategories());

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IngredientCategoryReadDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IngredientCategoryReadDto>> Get([FromRoute] Guid id) => HandleResult(await ingredientCategoryService.GetIngredientCategory(id));

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(IngredientCategoryReadDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IngredientCategoryReadDto>> Update([FromRoute] Guid id, [FromBody] IngredientCategoryUpdateDto dto) => HandleResult(await ingredientCategoryService.UpdateIngredientCategory(dto, id));

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete([FromRoute] Guid id) => HandleResult(await ingredientCategoryService.DeleteIngredientCategory(id));
}
