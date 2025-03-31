using Application.Dtos.MenuItems;
using Application.Dtos.OrderItemIngredients;

namespace Application.Dtos.OrderItems;

public class OrderItemReadDto
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public string? SpecialInstructions { get; set; }
    public decimal Discount { get; set; }

    public List<OrderItemIngredientReadDto> ExtraIngredients { get; set; } = new();
    public List<OrderItemIngredientReadDto> RemovedIngredients { get; set; } = new();

    public MenuItemDetailedDto MenuItem { get; set; } = null!;
}
