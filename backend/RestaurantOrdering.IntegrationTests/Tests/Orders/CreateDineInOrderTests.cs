using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Net;

public class CreateDineInOrderTests : IClassFixture<TestWebAppFactory>, IAsyncLifetime
{
    private readonly TestWebAppFactory _factory;
    private readonly HttpClient _client;

    public CreateDineInOrderTests(TestWebAppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_Create_DineIn_Order_With_Extras_And_Removals_And_Update_Table_Status()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RestaurantOrderingContext>();
        var data = new TestData(db);

        var areaId = await data.AreaAsync("Sala 1");
        var tableId = await data.TableAsync(areaId, "P2");

        var catId = await data.MenuCategoryAsync("Pizza");
        var ingCheese = await data.IngredientAsync("Cheese", 2m, true);
        var ingHam = await data.IngredientAsync("Ham", 4m, true);
        var ingOnion = await data.IngredientAsync("Onion", 1m, true);

        var pizzaId = await data.MenuItemWithBaseIngredientsAsync("Prosciutto", 20m, catId, ingCheese, ingHam);

        var payload = new
        {
            tableId,
            orderItems = new[]
            {
                new {
                    menuItemId = pizzaId,
                    discount = (decimal?)null,
                    specialInstructions = "well done",
                    extraIngredients = new[] { new { ingredientId = ingOnion, quantity = 2 } },
                    removedIngredientIds = new[] { ingHam }
                }
            }
        };

        var resp = await _client.PostAsJsonAsync("/api/orders/dinein", payload);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await resp.Content.ReadFromJsonAsync<OrderReadDto>();
        dto.Should().NotBeNull();
        dto!.TotalAmount.Should().Be(22m);

        // DB verify
        var saved = await db.Orders.Include(o => o.OrderItems).FirstAsync(o => o.Id == dto.Id);
        saved.TotalAmount.Should().Be(22m);

        var table = await db.Tables.FirstAsync(t => t.Id == tableId);
        await db.Entry(table).ReloadAsync();
        table.Status.Should().Be(Domain.TableStatus.PendingServingOrderItems);
    }

    // minimalne DTO do odczytu zwrotki
    private sealed class OrderReadDto
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? TableId { get; set; }
        public List<OrderItemReadDto> OrderItems { get; set; } = new();
    }
    private sealed class OrderItemReadDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public List<OrderItemIngredientReadDto> ExtraIngredients { get; set; } = new();
        public List<OrderItemIngredientReadDto> RemovedIngredients { get; set; } = new();
    }
    private sealed class OrderItemIngredientReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await DbReset.ResetAsync(_factory.Services);
}
