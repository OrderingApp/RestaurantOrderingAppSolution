using Domain;

namespace Application.Dtos.OrderItems;

public class OrderItemsListDto
{
    public Guid Id { get; set; }
    public string MenuItemName { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }

    public OrderItemStatus Status { get; set; }
}
