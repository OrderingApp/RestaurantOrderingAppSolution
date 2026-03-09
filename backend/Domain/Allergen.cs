namespace Domain;

public class Allergen
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<IngredientAllergenRel> IngredientAllergenRels { get; set; } = new();
}
