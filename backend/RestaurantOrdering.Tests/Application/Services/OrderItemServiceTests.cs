using Application.Dtos.MenuItems;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.OrderItems;
using RestaurantOrdering.Tests.TestData;
using System.Net;

public class OrderItemServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly OrderItemService _service;

    public OrderItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockMapper = new Mock<IMapper>();
        _mockEventHandler = new Mock<IEventHandlerService>();
        _service = new OrderItemService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddOrderItems_ShouldReturnSuccess_WhenValid()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var menuItem = MenuItemTestData.CreateMenuItem();
        var ingredient = IngredientTestData.CreateValidIngredient();

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        var dto = OrderItemTestData.CreateCreateDto(menuItem.Id);

        var mappedOrderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItem.Id,
            OrderId = order.Id,
            Price = menuItem.Price
        };

        _mockMapper.Setup(m => m.Map<OrderItem>(dto)).Returns(mappedOrderItem);

        _mockMapper
            .Setup(m => m.Map<OrderItemAddedEvent>(It.Is<(Guid, List<OrderItem>)>(tuple =>
                tuple.Item1 == order.Id
            )))
            .Returns((ValueTuple<Guid, List<OrderItem>> tuple) => new OrderItemAddedEvent
            {
                OrderId = tuple.Item1,
                OrderItemIds = tuple.Item2.Select(oi => oi.Id).ToList()
            });

        _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>()))
            .Returns(new OrderReadDto { Id = order.Id });

        // Act
        var result = await _service.AddOrderItemsToOrder(order.Id, [dto]);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(order.Id);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderItemAddedEvent>()), Times.Once);
    }

    [Fact]
    public async Task AddOrderItems_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var fakeOrderId = Guid.NewGuid();
        var dto = OrderItemTestData.CreateCreateDto(Guid.NewGuid());

        // Act
        var result = await _service.AddOrderItemsToOrder(fakeOrderId, [dto]);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Order not found.");
    }

    [Fact]
    public async Task AddOrderItems_ShouldReturnBadRequest_WhenMenuItemDoesNotExist()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var dto = OrderItemTestData.CreateCreateDto(Guid.NewGuid());

        // Act
        var result = await _service.AddOrderItemsToOrder(order.Id, [dto]);

        // Assert
        result.ShouldFailWith(HttpStatusCode.BadRequest, $"MenuItem with ID {dto.MenuItemId} not found.");
    }

    [Fact]
    public async Task AddOrderItems_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderItemService(null!, _mockEventHandler.Object, _mockMapper.Object);
        var result = await service.AddOrderItemsToOrder(Guid.NewGuid(), []);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task GetOrderItem_ShouldReturnSuccess_WhenItemExists()
    {
        // Arrange
        var menuItem = MenuItemTestData.CreateMenuItem();
        var order = OrderTestData.CreateOrder();
        var orderItem = OrderItemTestData.CreateOrderItem(orderId: order.Id, menuItemId: menuItem.Id);

        _dbContext.MenuItems.Add(menuItem);
        _dbContext.Orders.Add(order);
        _dbContext.OrderItems.Add(orderItem);

        await _dbContext.SaveChangesAsync();

        var readDto = new OrderItemReadDto
        {
            Id = orderItem.Id,
            Price = orderItem.Price,
            SpecialInstructions = orderItem.SpecialInstructions,
            Discount = orderItem.Discount,
            ExtraIngredients = [],
            RemovedIngredients = [],
            MenuItem = new MenuItemDetailedDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
            }
        };

        _mockMapper.Setup(m => m.Map<OrderItemReadDto>(It.Is<OrderItem>(oi => oi.Id == orderItem.Id)))
                       .Returns(readDto);

        // Act
        var result = await _service.GetOrderItem(order.Id, orderItem.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(orderItem.Id);
    }

    [Fact]
    public async Task GetOrderItem_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        // Act
        var result = await _service.GetOrderItem(orderId, itemId);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Order item not found.");
    }

    [Fact]
    public async Task GetOrderItem_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderItemService(null!, _mockEventHandler.Object, _mockMapper.Object);
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        // Act
        var result = await service.GetOrderItem(orderId, itemId);

        // Assert
        result.ShouldFailWith(HttpStatusCode.InternalServerError,
            result.ErrorMessage ?? "An error occurred (unexpectedly missing error message)");
    }

    [Fact]
    public async Task GetOrderItems_ShouldReturnItems_WhenOrderExists()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var menuItem = MenuItemTestData.CreateMenuItem();
        var orderItem = OrderItemTestData.CreateOrderItem(order.Id, menuItem.Id);

        _dbContext.Orders.Add(order);
        _dbContext.MenuItems.Add(menuItem);
        _dbContext.OrderItems.Add(orderItem);
        await _dbContext.SaveChangesAsync();

        var dtoList = new List<OrderItemsListDto>
    {
        new OrderItemsListDto { Id = orderItem.Id, Price = orderItem.Price }
    };

        _mockMapper.Setup(m => m.Map<List<OrderItemsListDto>>(It.IsAny<List<OrderItem>>()))
            .Returns(dtoList);

        // Act
        var result = await _service.GetOrderItems(order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Should().ContainSingle();
        result.Data.First().Id.Should().Be(orderItem.Id);
    }

    [Fact]
    public async Task GetOrderItems_ShouldReturnEmptyList_WhenNoItemsExist()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<List<OrderItemsListDto>>(It.IsAny<List<OrderItem>>()))
            .Returns(new List<OrderItemsListDto>());

        // Act
        var result = await _service.GetOrderItems(order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetOrderItems_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderItemService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.GetOrderItems(Guid.NewGuid());

        // Assert
        result.ShouldFailWith(HttpStatusCode.InternalServerError, result.ErrorMessage!);
    }

    [Fact]
    public async Task UpdateOrderItem_ShouldSucceed_WhenValidUpdateIsMade()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient();
        var menuItem = MenuItemTestData.CreateMenuItem();
        var order = OrderTestData.CreateOrder();
        var orderItem = OrderItemTestData.CreateOrderItem(order.Id, menuItem.Id);
        orderItem.Order = order;
        orderItem.MenuItem = menuItem;

        _dbContext.Orders.Add(order);
        _dbContext.MenuItems.Add(menuItem);
        _dbContext.Ingredients.Add(ingredient);
        _dbContext.OrderItems.Add(orderItem);
        await _dbContext.SaveChangesAsync();

        var updateDto = new OrderItemUpdateDto
        {
            SpecialInstructions = "No onions",
            Discount = 10,
            ExtraIngredients = new()
        {
            new() { IngredientId = ingredient.Id, Quantity = 2 }
        },
            RemovedIngredientIds = new()
        };

        _mockMapper.Setup(m => m.Map<OrderItemUpdatedEvent>(orderItem))
            .Returns(new OrderItemUpdatedEvent());

        _mockMapper.Setup(m => m.Map<OrderItemReadDto>(orderItem))
            .Returns(new OrderItemReadDto
            {
                Id = orderItem.Id,
                SpecialInstructions = orderItem.SpecialInstructions
            });


        // Act
        var result = await _service.UpdateOrderItem(order.Id, orderItem.Id, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(orderItem.Id);
        result.Data.SpecialInstructions.Should().Be("No onions");
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderItemUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderItem_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var updateDto = new OrderItemUpdateDto { SpecialInstructions = "None" };
        var fakeOrderId = Guid.NewGuid();
        var fakeItemId = Guid.NewGuid();

        // Act
        var result = await _service.UpdateOrderItem(fakeOrderId, fakeItemId, updateDto);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Order item not found.");
    }

    [Fact]
    public async Task UpdateOrderItem_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var updateDto = new OrderItemUpdateDto();
        var brokenService = new OrderItemService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await brokenService.UpdateOrderItem(Guid.NewGuid(), Guid.NewGuid(), updateDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task UpdateOrderItemStatus_ShouldSucceed_WhenValid()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var menuItem = MenuItemTestData.CreateMenuItem();
        var orderItem = OrderItemTestData.CreateOrderItem(order.Id, menuItem.Id);

        orderItem.Order = order;
        orderItem.MenuItem = menuItem;

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.OrderItems.AddAsync(orderItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m =>
            m.Map<OrderItemStatusUpdatedEvent>(It.Is<(OrderItem, OrderItemStatus)>(tuple =>
                tuple.Item1.Id == orderItem.Id && tuple.Item2 == OrderItemStatus.Pending
            ))
        ).Returns(new OrderItemStatusUpdatedEvent());

        // Act
        var result = await _service.UpdateOrderItemStatus(order.Id, orderItem.Id, OrderItemStatus.Served);

        Console.WriteLine(result.ErrorMessage);
        Console.WriteLine(result.HttpStatusCode);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeTrue();
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderItemStatusUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderItemStatus_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Act
        var result = await _service.UpdateOrderItemStatus(Guid.NewGuid(), Guid.NewGuid(), OrderItemStatus.Served);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Order item not found.");
    }

    [Fact]
    public async Task UpdateOrderItemStatus_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderItemService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.UpdateOrderItemStatus(Guid.NewGuid(), Guid.NewGuid(), OrderItemStatus.Served);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task DeleteOrderItem_ShouldSucceed_WhenValid()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var menuItem = MenuItemTestData.CreateMenuItem();
        var orderItem = OrderItemTestData.CreateOrderItem(order.Id, menuItem.Id);
        orderItem.Order = order;
        orderItem.MenuItem = menuItem;

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.OrderItems.AddAsync(orderItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper
            .Setup(m => m.Map<OrderItemDeletedEvent>(It.IsAny<OrderItem>()))
            .Returns(new OrderItemDeletedEvent());


        // Act
        var result = await _service.DeleteOrderItem(order.Id, orderItem.Id);

        Console.WriteLine(result.HttpStatusCode);
        Console.WriteLine(result.ErrorMessage);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeTrue();

        var deletedItem = await _dbContext.OrderItems.FindAsync(orderItem.Id);
        deletedItem.Should().BeNull();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderItemDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOrderItem_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Act
        var result = await _service.DeleteOrderItem(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Order item not found.");
    }

    [Fact]
    public async Task DeleteOrderItem_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderItemService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.DeleteOrderItem(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }
}
