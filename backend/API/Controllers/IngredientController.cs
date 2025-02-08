using Application.Contracts;
using Application.Dtos.Ingredients;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class IngredientsController(IIngredientService ingredientService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateIngredient([FromBody] IngredientCreateDto ingredientCreateDto) =>
        HandleResult(await ingredientService.CreateIngredient(ingredientCreateDto));

    [HttpGet]
    public async Task<IActionResult> GetAllIngredients() =>
        HandleResult(await ingredientService.GetAllIngredients());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIngredient(Guid id) =>
        HandleResult(await ingredientService.GetIngredient(id));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredient([FromBody] IngredientUpdateDto ingredientUpdateDto, Guid id) =>
        HandleResult(await ingredientService.UpdateIngredient(ingredientUpdateDto, id));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(Guid id) =>
        HandleResult(await ingredientService.DeleteIngredient(id));
}
