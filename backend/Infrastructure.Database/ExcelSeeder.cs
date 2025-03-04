using Domain;
using Infrastructure.Database;
using OfficeOpenXml;

public class ExcelSeeder
{
    private readonly RestaurantOrderingContext _context;

    public ExcelSeeder(RestaurantOrderingContext context)
    {
        _context = context;
    }

    public async Task SeedFromExcel(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(new FileInfo(filePath));

        await SeedMenuCategories(package);
        await SeedSubCategories(package);
        await SeedMenuItems(package);
        await SeedIngredients(package);
        await SeedTags(package);
        await SeedTables(package);
        await SeedOrders(package);
        await SeedOrderItems(package);
        await SeedCustomerInformation(package);
        await SeedReservations(package);
        await SeedIngredientTagRels(package);
        await SeedMenuItemIngredientRels(package);

        Console.WriteLine("Seeding complete!");
    }

    private async Task SeedMenuCategories(ExcelPackage package)
    {
        try
        {
            if (!_context.MenuCategories.Any())
            {
                var sheet = package.Workbook.Worksheets["MenuCategory"];
                var entities = new List<MenuCategory>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new MenuCategory
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Name = sheet.Cells[row, 2].Text,
                        IsUsed = TryParseBool(sheet.Cells[row, 3].Text),
                        IsDeleted = TryParseBool(sheet.Cells[row, 4].Text)
                    });
                }

                await _context.MenuCategories.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("MenuCategories seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding MenuCategories: {ex.Message}");
        }
    }

    private async Task SeedSubCategories(ExcelPackage package)
    {
        try
        {
            if (!_context.SubCategories.Any())
            {
                var sheet = package.Workbook.Worksheets["SubCategory"];
                var entities = new List<SubCategory>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new SubCategory
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Name = sheet.Cells[row, 2].Text,
                        IsUsed = TryParseBool(sheet.Cells[row, 3].Text),
                        IsDeleted = TryParseBool(sheet.Cells[row, 4].Text),
                        MenuCategoryId = TryParseGuid(sheet.Cells[row, 5].Text) ?? throw new Exception($"Invalid GUID in MenuCategoryId at row {row}")
                    });
                }

                await _context.SubCategories.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("SubCategories seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding SubCategories: {ex.Message}");
        }
    }

    private async Task SeedMenuItems(ExcelPackage package)
    {
        try
        {
            if (!_context.MenuItems.Any())
            {
                var sheet = package.Workbook.Worksheets["MenuItem"];
                var entities = new List<MenuItem>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new MenuItem
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Name = sheet.Cells[row, 2].Text,
                        Description = string.IsNullOrWhiteSpace(sheet.Cells[row, 3].Text) ? null : sheet.Cells[row, 3].Text,
                        Price = TryParseDecimal(sheet.Cells[row, 4].Text),
                        IsUsed = TryParseBool(sheet.Cells[row, 5].Text),
                        IsDeleted = TryParseBool(sheet.Cells[row, 6].Text),
                        MenuCategoryId = TryParseGuid(sheet.Cells[row, 7].Text),
                        SubCategoryId = TryParseGuid(sheet.Cells[row, 8].Text)
                    });
                }

                await _context.MenuItems.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("MenuItems seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding MenuItems: {ex.Message}");
        }
    }

    private async Task SeedIngredients(ExcelPackage package)
    {
        try
        {
            if (!_context.Ingredients.Any())
            {
                var sheet = package.Workbook.Worksheets["Ingredient"];
                var entities = new List<Ingredient>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new Ingredient
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Name = sheet.Cells[row, 2].Text,
                        Price = TryParseDecimal(sheet.Cells[row, 3].Text),
                        CanBeUsedAsExtra = TryParseBool(sheet.Cells[row, 4].Text),
                        IsDeleted = TryParseBool(sheet.Cells[row, 5].Text)
                    });
                }

                await _context.Ingredients.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("Ingredients seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Ingredients: {ex.Message}");
        }
    }

    private async Task SeedMenuItemIngredientRels(ExcelPackage package)
    {
        try
        {
            if (!_context.MenuItemIngredientRels.Any())
            {
                var sheet = package.Workbook.Worksheets["MenuItemIngredientRel"];
                var entities = new List<MenuItemIngredientRel>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    var menuItemId = TryParseGuid(sheet.Cells[row, 1].Text);
                    var ingredientId = TryParseGuid(sheet.Cells[row, 2].Text);

                    // 🔹 Ensure the IDs exist before inserting
                    if (menuItemId == null || ingredientId == null)
                        continue;

                    bool menuItemExists = _context.MenuItems.Any(m => m.Id == menuItemId);
                    bool ingredientExists = _context.Ingredients.Any(i => i.Id == ingredientId);

                    if (!menuItemExists || !ingredientExists)
                    {
                        Console.WriteLine($"Skipping row {row}: MenuItem or Ingredient does not exist.");
                        continue;
                    }

                    entities.Add(new MenuItemIngredientRel
                    {
                        MenuItemId = menuItemId.Value,
                        IngredientId = ingredientId.Value
                    });
                }

                await _context.MenuItemIngredientRels.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("MenuItemIngredientRels seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding MenuItemIngredientRels: {ex.Message}");
        }
    }


    private async Task SeedIngredientTagRels(ExcelPackage package)
    {
        try
        {
            if (!_context.IngredientTagRels.Any())
            {
                var sheet = package.Workbook.Worksheets["IngredientTagRel"];
                var entities = new List<IngredientTagRel>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    var ingredientId = TryParseGuid(sheet.Cells[row, 1].Text);
                    var tagId = TryParseGuid(sheet.Cells[row, 2].Text);

                    if (ingredientId == null || tagId == null)
                        continue;

                    bool ingredientExists = _context.Ingredients.Any(i => i.Id == ingredientId);
                    bool tagExists = _context.Tags.Any(t => t.Id == tagId);

                    if (!ingredientExists || !tagExists)
                    {
                        Console.WriteLine($"Skipping row {row}: Ingredient or Tag does not exist.");
                        continue;
                    }

                    entities.Add(new IngredientTagRel
                    {
                        IngredientId = ingredientId.Value,
                        TagId = tagId.Value
                    });
                }

                await _context.IngredientTagRels.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("IngredientTagRels seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding IngredientTagRels: {ex.Message}");
        }
    }

    private async Task SeedOrders(ExcelPackage package)
    {
        try
        {
            if (!_context.Orders.Any())
            {
                var sheet = package.Workbook.Worksheets["Order"];
                var entities = new List<Order>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new Order
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        DateTime = DateTime.Parse(sheet.Cells[row, 2].Text),
                        TotalAmount = TryParseDecimal(sheet.Cells[row, 3].Text),
                        Discount = TryParseDecimal(sheet.Cells[row, 4].Text),
                        DeliveryPrice = TryParseDecimal(sheet.Cells[row, 5].Text),
                        Status = TryParseEnum<OrderStatus>(sheet.Cells[row, 6].Text) ?? OrderStatus.Ongoing,
                        Type = TryParseEnum<OrderType>(sheet.Cells[row, 7].Text) ?? OrderType.DineIn,
                        TableId = TryParseGuid(sheet.Cells[row, 8].Text),
                        CustomerInformationId = TryParseGuid(sheet.Cells[row, 9].Text)
                    });
                }

                await _context.Orders.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("Orders seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Orders: {ex.Message}");
        }
    }

    private async Task SeedOrderItems(ExcelPackage package)
    {
        try
        {
            if (!_context.OrderItems.Any())
            {
                var sheet = package.Workbook.Worksheets["OrderItem"];
                var entities = new List<OrderItem>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new OrderItem
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Price = TryParseDecimal(sheet.Cells[row, 2].Text),
                        Discount = TryParseDecimal(sheet.Cells[row, 3].Text),
                        SpecialInstructions = sheet.Cells[row, 4].Text,
                        Status = TryParseEnum<OrderItemStatus>(sheet.Cells[row, 5].Text) ?? OrderItemStatus.Pending,
                        OrderId = TryParseGuid(sheet.Cells[row, 6].Text) ?? Guid.NewGuid(),
                        MenuItemId = TryParseGuid(sheet.Cells[row, 7].Text) ?? throw new Exception($"Invalid GUID in MenuItemId at row {row}")
                    });
                }

                await _context.OrderItems.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("OrderItems seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding OrderItems: {ex.Message}");
        }
    }

    private async Task SeedReservations(ExcelPackage package)
    {
        try
        {
            if (!_context.Reservations.Any())
            {
                var sheet = package.Workbook.Worksheets["Reservation"];
                var entities = new List<Reservation>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new Reservation
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        PhoneNumber = sheet.Cells[row, 2].Text,
                        Name = sheet.Cells[row, 3].Text,
                        DateTime = DateTime.Parse(sheet.Cells[row, 4].Text),
                        CapacityNeeded = int.Parse(sheet.Cells[row, 5].Text),
                        IsAssigned = TryParseBool(sheet.Cells[row, 6].Text),
                        TableId = TryParseGuid(sheet.Cells[row, 7].Text)
                    });
                }

                await _context.Reservations.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("Reservations seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Reservations: {ex.Message}");
        }
    }

    private async Task SeedTags(ExcelPackage package)
    {
        try
        {
            if (!_context.Tags.Any())
            {
                var sheet = package.Workbook.Worksheets["Tag"];
                var entities = new List<Tag>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new Tag
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Name = sheet.Cells[row, 2].Text?.Trim(),
                        IsUsed = TryParseBool(sheet.Cells[row, 3].Text),
                        IsDeleted = TryParseBool(sheet.Cells[row, 4].Text)
                    });
                }

                await _context.Tags.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("Tags seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Tags: {ex.Message}");
        }
    }

    private async Task SeedTables(ExcelPackage package)
    {
        try
        {
            if (!_context.Tables.Any())
            {
                var sheet = package.Workbook.Worksheets["Table"];
                var entities = new List<Table>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new Table
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        Name = sheet.Cells[row, 2].Text,
                        Capacity = int.Parse(sheet.Cells[row, 3].Text),
                        IsUsed = TryParseBool(sheet.Cells[row, 4].Text),
                        IsDeleted = TryParseBool(sheet.Cells[row, 5].Text),
                        Status = TryParseEnum<TableStatus>(sheet.Cells[row, 6].Text) ?? TableStatus.Available
                    });
                }

                await _context.Tables.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("Tables seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding Tables: {ex.Message}");
        }
    }

    private async Task SeedCustomerInformation(ExcelPackage package)
    {
        try
        {
            if (!_context.CustomerInformation.Any())
            {
                var sheet = package.Workbook.Worksheets["CustomerInformation"];
                var entities = new List<CustomerInformation>();

                for (int row = 2; row <= sheet.Dimension.End.Row; row++)
                {
                    entities.Add(new CustomerInformation
                    {
                        Id = TryParseGuid(sheet.Cells[row, 1].Text) ?? Guid.NewGuid(),
                        PhoneNumber = sheet.Cells[row, 2].Text,
                        AdditionalInstructions = sheet.Cells[row, 3].Text,
                        Address = sheet.Cells[row, 4].Text,
                        ExpectedOrderCompletion = DateTime.Parse(sheet.Cells[row, 5].Text),
                        OrderCompletionType = TryParseEnum<OrderCompletionType>(sheet.Cells[row, 6].Text) ?? OrderCompletionType.Scheduled,
                        PreferredPaymentMethod = TryParseEnum<PreferredPaymentMethod>(sheet.Cells[row, 7].Text) ?? PreferredPaymentMethod.Cash,
                        OrderId = TryParseGuid(sheet.Cells[row, 8].Text) ?? throw new Exception($"Invalid GUID in OrderId at row {row}")
                    });
                }

                await _context.CustomerInformation.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                Console.WriteLine("CustomerInformation seeded successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding CustomerInformation: {ex.Message}");
        }
    }

    private static Guid? TryParseGuid(string value)
    {
        return Guid.TryParse(value, out var parsedGuid) ? parsedGuid : null;
    }

    private static bool TryParseBool(string value)
    {
        return bool.TryParse(value, out var parsedBool) ? parsedBool : value.Trim().Equals("1") || value.Trim().ToLower() == "true";
    }

    private static decimal TryParseDecimal(string value)
    {
        return decimal.TryParse(value, out var parsedDecimal) ? parsedDecimal : 0m;
    }

    private static TEnum? TryParseEnum<TEnum>(string value) where TEnum : struct
    {
        return Enum.TryParse(value, true, out TEnum result) ? result : (TEnum?)null;
    }

}
