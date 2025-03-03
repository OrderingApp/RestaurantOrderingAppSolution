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
                new() { Id = Guid.NewGuid(), Name = "Przystawki" },
                new() { Id = Guid.NewGuid(), Name = "Panuozzo" },
                new() { Id = Guid.NewGuid(), Name = "Piadina" },
                new() { Id = Guid.NewGuid(), Name = "Pizza" },
                new() { Id = Guid.NewGuid(), Name = "Calzone" },
                new() { Id = Guid.NewGuid(), Name = "Napoje" },
                new() { Id = Guid.NewGuid(), Name = "Sezon Letni" },
                new() { Id = Guid.NewGuid(), Name = "Sezon Zimowy" },
                new() { Id = Guid.NewGuid(), Name = "Alkohol" }
            };
            await context.MenuCategories.AddRangeAsync(menuCategories);

            // ✅ SubCategories
            var subCategories = new List<SubCategory>
            {
                new() { Id = Guid.NewGuid(), Name = "Rossa", MenuCategoryId = menuCategories[3].Id },
                new() { Id = Guid.NewGuid(), Name = "Bianca", MenuCategoryId = menuCategories[3].Id },
                new() { Id = Guid.NewGuid(), Name = "Special", MenuCategoryId = menuCategories[3].Id },
                new() { Id = Guid.NewGuid(), Name = "Ciepłe", MenuCategoryId = menuCategories[5].Id },
                new() { Id = Guid.NewGuid(), Name = "Zimne", MenuCategoryId = menuCategories[5].Id },
                new() { Id = Guid.NewGuid(), Name = "Piwo", MenuCategoryId = menuCategories[8].Id },
                new() { Id = Guid.NewGuid(), Name = "Bezalkoholowe", MenuCategoryId = menuCategories[8].Id },
                new() { Id = Guid.NewGuid(), Name = "Drinki", MenuCategoryId = menuCategories[8].Id }
            };
            await context.SubCategories.AddRangeAsync(subCategories);

            // ✅ MenuItems
            var menuItems = new List<MenuItem>
            {
                new() { Id = Guid.NewGuid(), Name = "Numer 1", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 2", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 3", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 4", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 5", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 6", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 7", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 8", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 9", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 10", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 11", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 12", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 13", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 14", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 15", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 16", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 17", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 18", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 19", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 20", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 21", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 22", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 23", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 24", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 25", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 27", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 28", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 29", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 30", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[0].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 31", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 32", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[1].Id  },
                new() { Id = Guid.NewGuid(), Name = "Numer 33", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id, SubCategoryId = subCategories[2].Id  },
                new() { Id = Guid.NewGuid(), Name = "Podpłomyki", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Miska Buratty", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Deska Piccolo", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 1", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[1].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 2", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[1].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 1", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[2].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 2", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[2].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 3", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[2].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 31", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 32", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 33", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[3].Id },
                new() { Id = Guid.NewGuid(), Name = "Podpłomyki", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Miska Buratty", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Deska Piccolo", Price = 5.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[0].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 1", Price = 12.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[1].Id },
                new() { Id = Guid.NewGuid(), Name = "Numer 2", Price = 15.00M, IsUsed = true, IsDeleted = false, MenuCategoryId = menuCategories[1].Id },
            };
            await context.MenuItems.AddRangeAsync(menuItems);

            // ✅ Ingredients
            var ingredients = new List<Ingredient>
            {
                new() { Id = Guid.NewGuid(), Name = "Biały sos", Price = 1.5M },
                new() { Id = Guid.NewGuid(), Name = "Mozzarella", Price = 2M },
                new() { Id = Guid.NewGuid(), Name = "Pomidorki", Price = 1M },
                new() { Id = Guid.NewGuid(), Name = "Pesto", Price = 1.5M },
                new() { Id = Guid.NewGuid(), Name = "Nitki Chilli", Price = 1M },
                new() { Id = Guid.NewGuid(), Name = "Włoska szynka", Price = 3M },
                new() { Id = Guid.NewGuid(), Name = "Salami napoli", Price = 2.5M },
                new() { Id = Guid.NewGuid(), Name = "Kurczak", Price = 3M },
                new() { Id = Guid.NewGuid(), Name = "Łosoś", Price = 3.5M },
                new() { Id = Guid.NewGuid(), Name = "Czerwona cebula", Price = 1M },
                new() { Id = Guid.NewGuid(), Name = "Bazylia", Price = 0.5M },
                new() { Id = Guid.NewGuid(), Name = "Pieczarki", Price = 1M }
            };
            await context.Ingredients.AddRangeAsync(ingredients);

            // ✅ Tags
            var tags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "Szynka" },
                new() { Id = Guid.NewGuid(), Name = "Salami" },
                new() { Id = Guid.NewGuid(), Name = "Spianata" },
                new() { Id = Guid.NewGuid(), Name = "Kurczak" },
                new() { Id = Guid.NewGuid(), Name = "Wege" },
                new() { Id = Guid.NewGuid(), Name = "N'duja" },
                new() { Id = Guid.NewGuid(), Name = "Łosoś" }
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
