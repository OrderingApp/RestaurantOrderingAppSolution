using Domain;

namespace Application.Dtos.Ingredients;

public class IngredientCreateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public IngredientType IngredientType { get; set; }
}
