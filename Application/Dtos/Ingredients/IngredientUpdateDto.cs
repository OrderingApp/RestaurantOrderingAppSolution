namespace Application.Dtos.Ingredients;

public class IngredientUpdateDto
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? IngredientType { get; set; }
    public bool? IsUsed { get; set; }
}
