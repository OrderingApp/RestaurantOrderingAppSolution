namespace Application.Dtos.Ingredients;

public class IngredientReadDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Allergens { get; set; } = new();
}
