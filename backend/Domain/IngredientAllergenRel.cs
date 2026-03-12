namespace Domain;

public class IngredientAllergenRel
{
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public Guid AllergenId { get; set; }
    public Allergen Allergen { get; set; } = null!;
}
