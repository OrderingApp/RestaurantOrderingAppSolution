using Application.Dtos.OrderItems;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders;
using Application.Services;
using AutoMapper;
using Domain;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using FluentAssertions;
using System.Net;
using RestaurantOrdering.Tests.TestHelpers;

public class OrderServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
                    .UseInMemoryDatabase(databaseName: "TestDatabase")
                    .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _orderService = new OrderService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateDineInOrder_ShouldReturnSuccess_WhenTableExists()
    {
        // Arrange
        var table = TableTestHelper.CreateTable();
        var menuItem = MenuItemTestHelper.CreateMenuItem();
        var orderItem = OrderItemTestHelper.CreateOrderItem(menuItem.Id);
        var order = OrderTestHelper.CreateOrder(tableId: table.Id, items: new List<OrderItem> { orderItem });

        // ✅ Add required entities to in-memory DB
        _dbContext.Tables.Add(table);
        _dbContext.MenuItems.Add(menuItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<Order>(It.IsAny<DineInOrderCreateDto>())).Returns(order);

        // Act
        var result = await _orderService.CreateDineInOrder(new DineInOrderCreateDto
        {
            TableId = table.Id,
            OrderItems = new List<OrderItemCreateDto>
        {
            new OrderItemCreateDto { MenuItemId = menuItem.Id}
        }
        });

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
    }
}
