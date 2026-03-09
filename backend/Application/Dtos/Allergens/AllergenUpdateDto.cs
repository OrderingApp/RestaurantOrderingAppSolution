namespace Application.Dtos.Allergens;

public class AllergenUpdateDto
{
    public string Name { get; set; } = null!;

    /// <summary>
    /// Optional EU allergen number (1–14) per Annex II of the FIC Regulation.
    /// Set to null to remove the EU number association.
    /// </summary>
    public int? EuNumber { get; set; }

    public bool IsUsed { get; set; }
}
