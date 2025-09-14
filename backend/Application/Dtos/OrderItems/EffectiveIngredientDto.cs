namespace Application.Dtos.OrderItems;

public class EffectiveIngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }

    public bool FromBase { get; set; }
    public bool IsExtra { get; set; }
    public bool IsRemoved { get; set; }
    public decimal PriceDelta { get; set; }
}
