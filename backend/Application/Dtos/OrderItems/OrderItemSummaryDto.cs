using Application.Dtos.OrderItemIngredients;

namespace Application.Dtos.OrderItems;

public class OrderItemSummaryDto
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? SpecialInstructions { get; set; }
    public decimal Discount { get; set; }
    public string MenuItemName { get; set; } = null!;

    public List<OrderItemIngredientReadDto> Ingredients { get; set; } = new List<OrderItemIngredientReadDto>();
}
