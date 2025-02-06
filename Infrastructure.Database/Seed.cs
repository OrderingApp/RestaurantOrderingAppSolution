using Domain;

namespace Infrastructure.Database;

public class Seed
{
    public static async Task SeedData(RestaurantOrderingContext context)
    {
        if (!context.MenuCategories.Any())
        {
            // MenuCategories
            var menuCategories = new List<MenuCategory>
            {
                new MenuCategory { Id = Guid.NewGuid(), Name = "Appetizers", IsUsed = true, IsDeleted = false },
                new MenuCategory { Id = Guid.NewGuid(), Name = "Veg Pizza", IsUsed = true, IsDeleted = false },
                new MenuCategory { Id = Guid.NewGuid(), Name = "Meat Pizza", IsUsed = true, IsDeleted = false }
            };
            await context.MenuCategories.AddRangeAsync(menuCategories);

            // MenuItems
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Id = Guid.NewGuid(), Name = "Garlic Bread", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new MenuItem { Id = Guid.NewGuid(), Name = "Veggie Delight Pizza", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[1].Id },
                new MenuItem { Id = Guid.NewGuid(), Name = "Pepperoni Pizza", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[2].Id }
            };
            await context.MenuItems.AddRangeAsync(menuItems);

            // Ingredients
            var ingredients = new List<Ingredient>
            {
                new Ingredient { Id = Guid.NewGuid(), Name = "Mozzarella", Price = 1.5M, IngredientType = IngredientType.Cheese, IsUsed = true, IsDeleted = false },
                new Ingredient { Id = Guid.NewGuid(), Name = "Tomato", Price = 0.5M, IngredientType = IngredientType.Vegetables, IsUsed = true, IsDeleted = false },
                new Ingredient { Id = Guid.NewGuid(), Name = "Pepperoni", Price = 2.0M, IngredientType = IngredientType.Meat, IsUsed = true, IsDeleted = false }
            };
            await context.Ingredients.AddRangeAsync(ingredients);

            // Tags
            var tags = new List<Tag>
            {
                new Tag { Id = Guid.NewGuid(), Name = "Vegetarian", IsUsed = true, IsDeleted = false },
                new Tag { Id = Guid.NewGuid(), Name = "Spicy", IsUsed = true, IsDeleted = false }
            };
            await context.Tags.AddRangeAsync(tags);

            // MenuItemTags
            var menuItemTags = new List<MenuItemTag>
            {
                new MenuItemTag { MenuItemId = menuItems[1].Id, TagId = tags[0].Id },
                new MenuItemTag { MenuItemId = menuItems[2].Id, TagId = tags[1].Id }
            };
            await context.MenuItemTags.AddRangeAsync(menuItemTags);

            // Tables
            var tables = new List<Table>
            {
                new Table { Id = Guid.NewGuid(), Name = "Table 1", NumberOfPeople = 4, IsOccupied = false, IsUsed = true, IsDeleted = false },
                new Table { Id = Guid.NewGuid(), Name = "Table 2", NumberOfPeople = 2, IsOccupied = true, IsUsed = true, IsDeleted = false }
            };
            await context.Tables.AddRangeAsync(tables);

            // Orders
            var orders = new List<Order>
            {
                new Order
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
                new Order
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

            // OrderItems
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = menuItems[0].Id,
                    OrderId = orders[0].Id,
                    Quantity = 2,
                    Price = 5.00M * 2,
                    SpecialInstructions = "Extra cheese"
                },
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = menuItems[2].Id,
                    OrderId = orders[1].Id,
                    Quantity = 1,
                    Price = 15.00M,
                    SpecialInstructions = "Add extra pepperoni"
                }
            };
            await context.OrderItems.AddRangeAsync(orderItems);

            // OrderItemIngredients
            var orderItemIngredients = new List<OrderItemIngredient>
            {
                new OrderItemIngredient { OrderItemId = orderItems[0].Id, IngredientId = ingredients[0].Id, Quantity = 2 }, // Mozzarella
                new OrderItemIngredient { OrderItemId = orderItems[1].Id, IngredientId = ingredients[2].Id, Quantity = 1 }  // Pepperoni
            };
            await context.OrderItemIngredients.AddRangeAsync(orderItemIngredients);

            await context.SaveChangesAsync();
        }
    }
}
