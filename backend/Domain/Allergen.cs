namespace Domain;

public class Allergen
{
    public Guid Id { get; set; }
    public required string Name { get; set; }

    /// <summary>
    /// EU allergen number (1–14) per Annex II of the FIC Regulation.
    /// Null for allergens that are not part of the 14 mandatory EU allergens.
    /// </summary>
    public int? EuNumber { get; set; }

    public bool IsUsed { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public List<IngredientAllergenRel> IngredientAllergenRels { get; set; } = new();
}
