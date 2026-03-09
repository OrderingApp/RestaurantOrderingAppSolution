namespace Application.Dtos.Ingredients;

public class IngredientAllergenDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    /// <summary>
    /// EU allergen number (1–14) per Annex II of the FIC Regulation.
    /// Null if this allergen is not one of the 14 mandatory EU allergens.
    /// </summary>
    public int? EuNumber { get; set; }
}
