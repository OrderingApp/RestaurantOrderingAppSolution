using Domain;

namespace RestaurantOrdering.Tests.TestHelpers;

public static class OrderItemTestData
{
    public static OrderItem CreateOrderItem(
        Guid? menuItemId = null,
        decimal price = 10m,
        string? specialInstructions = null
    )
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItemId ?? Guid.NewGuid(),
            Price = price,
            SpecialInstructions = specialInstructions ?? "Default Special Instructions",
            ExtraIngredients = new List<OrderItemIngredient>(),
            RemovedIngredients = new List<OrderItemIngredient>(),
        };
    }
}
