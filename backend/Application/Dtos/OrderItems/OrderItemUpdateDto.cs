using Application.Dtos.OrderItemIngredients;

namespace Application.Dtos.OrderItems;

public class OrderItemUpdateDto
{
    public string? SpecialInstructions { get; set; }
    public decimal? Discount { get; set; }

    public List<OrderItemIngredientAddDto> ExtraIngredients { get; set; } = new();
    public List<Guid> RemovedIngredientIds { get; set; } = new();
}
