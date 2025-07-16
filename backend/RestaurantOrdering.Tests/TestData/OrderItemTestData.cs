using Application.Dtos.OrderItemIngredients;
using Application.Dtos.OrderItems;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class OrderItemTestData
{
    public static readonly Guid DefaultMenuItemId = Guid.NewGuid();
    public const decimal DefaultPrice = 10.0m;
    public const string DefaultSpecialInstructions = "No onions";

    public static OrderItemCreateDto CreateCreateDto(
        Guid? menuItemId = null,
        string? specialInstructions = DefaultSpecialInstructions,
        List<OrderItemIngredientAddDto>? extraIngredients = null,
        List<Guid>? removedIngredientIds = null
    )
    {
        return new OrderItemCreateDto
        {
            MenuItemId = menuItemId ?? DefaultMenuItemId,
            SpecialInstructions = specialInstructions,
            ExtraIngredients = extraIngredients ?? new(),
            RemovedIngredientIds = removedIngredientIds ?? new()
        };
    }

    public static OrderItem CreateOrderItem(
        Guid? id = null,
        Guid? orderId = null,
        Guid? menuItemId = null,
        decimal price = DefaultPrice,
        string? specialInstructions = DefaultSpecialInstructions,
        OrderItemStatus status = OrderItemStatus.Pending,
        List<OrderItemIngredient>? extraIngredients = null,
        List<OrderItemIngredient>? removedIngredients = null
    )
    {
        return new OrderItem
        {
            Id = id ?? Guid.NewGuid(),
            OrderId = orderId ?? Guid.NewGuid(),
            MenuItemId = menuItemId ?? DefaultMenuItemId,
            Price = price,
            Discount = 0,
            SpecialInstructions = specialInstructions,
            Status = status,
            ExtraIngredients = extraIngredients ?? new(),
            RemovedIngredients = removedIngredients ?? new()
        };
    }

    public static OrderItemIngredientAddDto CreateExtraIngredientAddDto(Guid? ingredientId = null, int quantity = 1)
    {
        return new OrderItemIngredientAddDto
        {
            IngredientId = ingredientId ?? Guid.NewGuid(),
            Quantity = quantity
        };
    }

    public static OrderItemIngredient CreateOrderItemIngredient(string name, decimal price, int quantity = 1)
    {
        return new OrderItemIngredient
        {
            Id = Guid.NewGuid(),
            Name = name,
            Price = price,
            Quantity = quantity
        };
    }
}
