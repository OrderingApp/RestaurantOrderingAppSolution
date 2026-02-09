using Application.Contracts;
using Application.Dtos.IngredientCategories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/ingredient-categories")]
public class IngredientCategoryController(IIngredientCategoryService ingredientCategoryService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IngredientCategoryCreateDto dto) => HandleResult(await ingredientCategoryService.CreateIngredientCategory(dto));

    [HttpGet]
    public async Task<IActionResult> GetAll() => HandleResult(await ingredientCategoryService.GetAllIngredientCategories());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id) => HandleResult(await ingredientCategoryService.GetIngredientCategory(id));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] IngredientCategoryUpdateDto dto, Guid id) => HandleResult(await ingredientCategoryService.UpdateIngredientCategory(dto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) => HandleResult(await ingredientCategoryService.DeleteIngredientCategory(id));
}
