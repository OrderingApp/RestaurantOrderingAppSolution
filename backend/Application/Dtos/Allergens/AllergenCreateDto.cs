namespace Application.Dtos.Allergens;

public class AllergenCreateDto
{
    public string Name { get; set; } = null!;

    /// <summary>
    /// Optional EU allergen number (1–14) per Annex II of the FIC Regulation.
    /// </summary>
    public int? EuNumber { get; set; }
}
