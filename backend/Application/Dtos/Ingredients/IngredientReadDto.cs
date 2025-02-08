namespace Application.Dtos.Ingredients;

public class IngredientReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string IngredientType { get; set; } = null!;
}
