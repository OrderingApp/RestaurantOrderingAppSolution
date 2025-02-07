using Application.Dtos.OrderItemIngredients;

namespace Application.Dtos.OrderItems
{
    public class OrderItemsListDto
    {
        public Guid Id { get; set; }
        public string? MenuItemName { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }

        public List<OrderItemIngredientReadDto> Ingredients { get; set; } = new List<OrderItemIngredientReadDto>();

    }
}
