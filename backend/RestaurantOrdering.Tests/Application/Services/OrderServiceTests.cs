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
        var tableId = Guid.NewGuid();
        var dineInOrderDto = new DineInOrderCreateDto
        {
            TableId = tableId,
            OrderItems = new List<OrderItemCreateDto>
        {
            new OrderItemCreateDto { MenuItemId = Guid.NewGuid() }
        }
        };

        var table = new Table { Id = tableId, Name = "test" };

        // ✅ Add the table to the in-memory database
        _dbContext.Tables.Add(table);
        await _dbContext.SaveChangesAsync();

        var orderEntity = new Order { Id = Guid.NewGuid(), TableId = tableId };
        var orderReadDto = new OrderReadDto { Id = orderEntity.Id };

        _mockMapper.Setup(m => m.Map<Order>(dineInOrderDto)).Returns(orderEntity);
        _mockMapper.Setup(m => m.Map<OrderReadDto>(orderEntity)).Returns(orderReadDto);

        // Act
        var result = await _orderService.CreateDineInOrder(dineInOrderDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
    }
}
