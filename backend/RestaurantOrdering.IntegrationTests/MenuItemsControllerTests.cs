using FluentAssertions;
using System.Net;

public class MenuItemsControllerTests : IClassFixture<TestWebAppFactory>
{
    private readonly HttpClient _client;

    public MenuItemsControllerTests(TestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Should_Return_Seeded_Item()
    {
        var resp = await _client.GetAsync("/api/menu-items");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await resp.Content.ReadFromJsonAsync<List<MenuItemDto>>();
        items.Should().NotBeNull();
        items!.Should().ContainSingle(i => i.Name == "Pizza" && i.Price == 12.5m);
    }

    private record MenuItemDto(Guid Id, string Name, decimal Price);
}
