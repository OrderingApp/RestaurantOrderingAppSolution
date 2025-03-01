using Domain;

namespace RestaurantOrdering.Tests.TestHelpers;

public static class OrderItemTestingContext
{
    public static OrderItem OrderItem1 = new OrderItem
    {
        Id = Guid.NewGuid(),
        MenuItemId = Guid.NewGuid(),
        Price = 10,
        SpecialInstructions = "Test item 1",
        //OrderItemIngredients = new List<MenuItemIngredientRel>()
    };
}
