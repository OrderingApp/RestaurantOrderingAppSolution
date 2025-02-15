using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class Seed
{
    public static async Task SeedData(RestaurantOrderingContext context)
    {
        if (!await context.MenuCategories.AnyAsync())
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

            // ✅ MenuItemIngredientRel
            var menuItemIngredientRels = new List<MenuItemIngredientRel>
            {
                new() { MenuItemId = menuItems[1].Id, IngredientId = ingredients[0].Id }, // Veggie Pizza -> Mozzarella
                new() { MenuItemId = menuItems[1].Id, IngredientId = ingredients[1].Id }, // Veggie Pizza -> Tomato
                new() { MenuItemId = menuItems[2].Id, IngredientId = ingredients[0].Id }, // Pepperoni Pizza -> Mozzarella
                new() { MenuItemId = menuItems[2].Id, IngredientId = ingredients[2].Id }  // Pepperoni Pizza -> Pepperoni
            };
            await context.MenuItemIngredientRels.AddRangeAsync(menuItemIngredientRels);

            // ✅ Tables
            var tables = new List<Table>
            {
                new() { Id = Guid.NewGuid(), Name = "Table 1", Capacity = 4, Status = TableStatus.Available, IsUsed = true, IsDeleted = false },
                new() { Id = Guid.NewGuid(), Name = "Table 2", Capacity = 2, Status = TableStatus.Ongoing, IsUsed = true, IsDeleted = false }
            };
            await context.Tables.AddRangeAsync(tables);

            // ✅ Orders
            var orders = new List<Order>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = DateTime.UtcNow,
                    Status = OrderStatus.Ongoing,
                    Type = OrderType.DineIn,
                    TableId = tables[0].Id,
                    CustomerInformation = new CustomerInformation
                    {
                        Id = Guid.NewGuid(),
                        PhoneNumber = "1234567890",
                        AdditionalInstructions = null,
                        Address = null,
                        ExpectedOrderCompletion = null,
                        OrderCompletionType = OrderCompletionType.Immediate,
                        PreferredPaymentMethod = PreferredPaymentMethod.Cash
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    DateTime = DateTime.UtcNow.AddHours(-2),
                    Status = OrderStatus.Ongoing,
                    Type = OrderType.Delivery,
                    CustomerInformation = new CustomerInformation
                    {
                        Id = Guid.NewGuid(),
                        PhoneNumber = "0987654321",
                        AdditionalInstructions = "Leave at the door",
                        Address = "123 Main Street",
                        ExpectedOrderCompletion = DateTime.UtcNow.AddMinutes(45),
                        OrderCompletionType = OrderCompletionType.Scheduled,
                        PreferredPaymentMethod = PreferredPaymentMethod.Card
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
                    Discount = 0,
                    SpecialInstructions = "Extra cheese",
                    Status = OrderItemStatus.Pending,
                    ExtraIngredients = new List<OrderItemIngredient>
                    {
                        new() { Id = Guid.NewGuid(), Name = "Mozzarella", Price = 1.5M, Quantity = 2 }
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = menuItems[2].Id,
                    OrderId = orders[1].Id,
                    Price = 15.00M,
                    Discount = 0,
                    SpecialInstructions = "Add extra pepperoni",
                    Status = OrderItemStatus.Pending,
                    ExtraIngredients = new List<OrderItemIngredient>
                    {
                        new() { Id = Guid.NewGuid(), Name = "Pepperoni", Price = 2.0M, Quantity = 1 }
                    }
                }
            };
            await context.OrderItems.AddRangeAsync(orderItems);

            // ✅ Payments
            var payments = new List<Payment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderId = orders[0].Id,
                    Amount = 10.00M,
                    PaidAt = DateTime.UtcNow,
                    IsRefunded = false,
                    PaymentMethod = PaymentMethod.Cash
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrderId = orders[1].Id,
                    Amount = 15.00M,
                    PaidAt = DateTime.UtcNow.AddMinutes(-30),
                    IsRefunded = false,
                    PaymentMethod = PaymentMethod.Card
                }
            };
            await context.Payments.AddRangeAsync(payments);

            // ✅ SalesRevenue
            var salesRevenue = new List<SalesRevenue>
            {
                new() { Id = Guid.NewGuid(), Amount = 100.00M, Date = DateTime.UtcNow.Date },
                new() { Id = Guid.NewGuid(), Amount = 200.00M, Date = DateTime.UtcNow.Date.AddDays(-1) }
            };
            await context.SalesRevenues.AddRangeAsync(salesRevenue);

            // ✅ Reservations
            var reservations = new List<Reservation>
            {
                new() { Id = Guid.NewGuid(), PhoneNumber = "123123123", Name = "Smith", DateTime = DateTime.UtcNow.AddHours(3), CapacityNeeded = 4, IsAssigned = false },
                new() { Id = Guid.NewGuid(), PhoneNumber = "987987987", Name = "Johnson", DateTime = DateTime.UtcNow.AddHours(5), CapacityNeeded = 2, IsAssigned = false }
            };
            await context.Reservations.AddRangeAsync(reservations);

            await context.SaveChangesAsync();
        }
    }
}
