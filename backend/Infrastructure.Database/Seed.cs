using Domain;

namespace Infrastructure.Database;

public class Seed
{
    public static async Task SeedData(RestaurantOrderingContext context)
    {
        if (!context.MenuCategories.Any())
        {
            // ✅ MenuCategories
            var menuCategories = new List<MenuCategory>
            {
                new() { Id = Guid.NewGuid(), Name = "Appetizers", IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Veg Pizza", IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Meat Pizza", IsUsed = true, IsDeleted = false }
            };
            await context.MenuCategories.AddRangeAsync(menuCategories);

            // ✅ MenuItems
            var menuItems = new List<MenuItem>
            {
                new() { Id = Guid.NewGuid(), Name = "Garlic Bread", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Veggie Delight Pizza", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[1].Id },
                new() { Id = Guid.NewGuid(), Name = "Pepperoni Pizza", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[2].Id }
            };
            await context.MenuItems.AddRangeAsync(menuItems);

            // ✅ Ingredients
            var ingredients = new List<Ingredient>
            {
                new() { Id = Guid.NewGuid(), Name = "Mozzarella", Price = 1.5M, IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Tomato", Price = 0.5M, IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Pepperoni", Price = 2.0M, IsUsed = true, IsDeleted = false }
            };
            await context.Ingredients.AddRangeAsync(ingredients);

            // ✅ Tags
            var tags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Vegetarian", IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Spicy", IsUsed = true, IsDeleted = false }
            };
            await context.Tags.AddRangeAsync(tags);

            // ✅ Tables (Fixed Capacity & TableStatus)
            var tables = new List<Table>
            {
                new() { Id = Guid.NewGuid(), Name = "Table 1", Capacity = 4, TableStatus = TableStatus.Available, IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Table 2", Capacity = 2, TableStatus = TableStatus.Ongoing, IsUsed = true, IsDeleted = false }
            };
            await context.Tables.AddRangeAsync(tables);

            // ✅ Orders
            var orders = new List<Order>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderDateTime = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Ongoing,
                    OrderType = OrderType.DineIn,
                    TableId = tables[0].Id,
                    CustomerInformation = new CustomerInformation
                    {
                        Id = Guid.NewGuid(),
                        PhoneNumber = "1234567890",
                        AdditionalInstructions = null,
                        Address = null
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderDateTime = DateTime.UtcNow.AddHours(-2),
                    OrderStatus = OrderStatus.Ongoing,
                    OrderType = OrderType.Delivery,
                    CustomerInformation = new CustomerInformation
                    {
                        Id = Guid.NewGuid(),
                        PhoneNumber = "0987654321",
                        AdditionalInstructions = "Leave at the door",
                        Address = "123 Main Street"
                    }
                }
            };
            await context.Orders.AddRangeAsync(orders);

            // ✅ OrderItems
            var orderItems = new List<OrderItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = menuItems[0].Id,
                    OrderId = orders[0].Id,
                    Price = 5.00M * 2,
                    SpecialInstructions = "Extra cheese",
                    Status = OrderItemStatus.Pending
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = menuItems[2].Id,
                    OrderId = orders[1].Id,
                    Price = 15.00M,
                    SpecialInstructions = "Add extra pepperoni",
                    Status = OrderItemStatus.Pending
                }
            };
            await context.OrderItems.AddRangeAsync(orderItems);

            // ✅ OrderItemIngredients (Fixed Relationship)
            var orderItemIngredients = new List<OrderItemIngredient>
            {
                new()
                {
                    OrderItemId = orderItems[0].Id,
                    IngredientId = ingredients[0].Id, // Mozzarella
                    Price = 1.5M,
                    Quantity = 2
                },
                new()
                {
                    OrderItemId = orderItems[1].Id,
                    IngredientId = ingredients[2].Id, // Pepperoni
                    Price = 2.0M,
                    Quantity = 1
                }
            };
            await context.OrderItemIngredients.AddRangeAsync(orderItemIngredients);

            await context.SaveChangesAsync();
        }
    }
}
