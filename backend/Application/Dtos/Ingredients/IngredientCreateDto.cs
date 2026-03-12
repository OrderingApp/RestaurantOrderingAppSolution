namespace Application.Dtos.Ingredients;

public class IngredientCreateDto
{
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid? CategoryId { get; set; }
    //public List<Guid> TagIds { get; set; } = new();
}
