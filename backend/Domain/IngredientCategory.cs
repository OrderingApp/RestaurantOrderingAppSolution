namespace Domain;

public class IngredientCategory
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<Ingredient> Ingredients { get; set; } = new();
}
