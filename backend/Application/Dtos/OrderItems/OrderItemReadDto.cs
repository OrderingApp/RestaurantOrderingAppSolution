using Application.Dtos.OrderItemIngredients;

namespace Application.Dtos.OrderItems;

public class OrderItemReadDto
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? SpecialInstructions { get; set; }
    public decimal Discount { get; set; }

    public List<OrderItemIngredientReadDto> Ingredients { get; set; } = new List<OrderItemIngredientReadDto>();

    public Guid OrderId { get; set; }
    public Guid MenuItemId { get; set; }

    public string? MenuItemName { get; set; }
}
