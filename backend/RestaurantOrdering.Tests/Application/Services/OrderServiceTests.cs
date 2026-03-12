using Application.Contracts;
using Application.Dtos.CustomerInformations;
using Application.Dtos.OrderItemIngredients;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders;
using Application.Dtos.Orders.OrderDineIn;
using Application.Dtos.Orders.OrderTakeAway;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Orders.CreatingOrder;
using RestaurantOrdering.Events.Domain.Orders.DeleteOrder;
using RestaurantOrdering.Events.Domain.Orders.DiscountsOrder;
using RestaurantOrdering.Events.Domain.Orders.ModificationsOrder;
using RestaurantOrdering.Events.Domain.Orders.PaymentsOrder;
using RestaurantOrdering.Tests.TestData;
using System.Net;
public class OrderServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new OrderService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateDineInOrder_ShouldSucceed_WhenValidInput()
    {
        // Arrange
        var table = TableTestData.CreateCorrectTable(); // make sure it sets Id and Status
        var menuItem = MenuItemTestData.CreateMenuItem(); // make sure it sets Id and Price
        var ingredient = IngredientTestData.CreateValidIngredient();

        var orderItemDto = new OrderItemCreateDto
        {
            MenuItemId = menuItem.Id,
            SpecialInstructions = "Extra spicy",
            ExtraIngredients = new List<OrderItemIngredientAddDto>
        {
            new() { IngredientId = ingredient.Id, Quantity = 1 }
        },
            RemovedIngredientIds = new List<Guid> { }
        };

        var dineInOrderDto = new DineInOrderCreateDto
        {
            TableId = table.Id,
            OrderItems = new List<OrderItemCreateDto> { orderItemDto },
        };

        await _dbContext.Tables.AddAsync(table);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<Order>(It.IsAny<DineInOrderCreateDto>()))
            .Returns<DineInOrderCreateDto>(dto => new Order
            {
                Id = Guid.NewGuid(),
                TableId = dto.TableId,
                Type = OrderType.DineIn
            });

        _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>()))
            .Returns<Order>(order => new OrderReadDto
            {
                Id = order.Id,
                TableId = order.TableId,
                TotalAmount = order.TotalAmount,
                OrderType = order.Type.ToString(),
                OrderStatus = order.Status.ToString()
            });

        _mockMapper.Setup(m => m.Map<DineInOrderCreatedEvent>(It.IsAny<Order>()))
            .Returns(new DineInOrderCreatedEvent());

        // Act
        var result = await _service.CreateDineInOrder(dineInOrderDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data.Should().NotBeNull();
        result.Data!.TableId.Should().Be(dineInOrderDto.TableId);
        result.Data.OrderType.Should().Be(OrderType.DineIn.ToString());

        var orderInDb = await _dbContext.Orders.FirstOrDefaultAsync(o => o.TableId == table.Id);
        orderInDb.Should().NotBeNull();
        orderInDb!.OrderItems.Should().HaveCount(1);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<DineInOrderCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateDineInOrder_ShouldFail_WhenTableDoesNotExist()
    {
        // Arrange
        var nonExistentTableId = Guid.NewGuid();

        var createDto = new DineInOrderCreateDto
        {
            TableId = nonExistentTableId,
            OrderItems = new List<OrderItemCreateDto>()
        };

        // Act
        var result = await _service.CreateDineInOrder(createDto);

        // Assert
        result.ShouldFailWith(
            HttpStatusCode.BadRequest,
            "Specified table does not exist."
        );

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<DineInOrderCreatedEvent>()), Times.Never);
    }

    [Fact]
    public async Task CreateTakeawayOrder_ShouldSucceed_WhenValid()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient();
        var menuItem = MenuItemTestData.CreateMenuItem();

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SaveChangesAsync();

        var dto = OrderTestData.CreateTakeawayOrderCreateDto(menuItem.Id, ingredient.Id);

        _mockMapper.Setup(m => m.Map<Order>(It.IsAny<TakeawayOrderCreateDto>()))
            .Returns(new Order
            {
                Id = Guid.NewGuid(),
                Type = OrderType.Takeaway
            });

        _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>()))
            .Returns(new OrderReadDto());

        _mockMapper.Setup(m => m.Map<TakeawayOrderCreatedEvent>(It.IsAny<Order>()))
            .Returns(new TakeawayOrderCreatedEvent());

        // Act
        var result = await _service.CreateTakeawayOrder(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TakeawayOrderCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateTakeawayOrder_ShouldFail_WhenMenuItemDoesNotExist()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient();
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        // Use a non-existent MenuItemId
        var invalidMenuItemId = Guid.NewGuid();
        var dto = OrderTestData.CreateTakeawayOrderCreateDto(invalidMenuItemId, ingredient.Id);

        // Act
        var result = await _service.CreateTakeawayOrder(dto);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Object reference not set to an instance of an object."
        );
    }

    [Fact]
    public async Task CreateDeliveryOrder_ShouldSucceed_WhenValid()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient();
        var menuItem = MenuItemTestData.CreateMenuItem();

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SaveChangesAsync();

        var dto = OrderTestData.CreateDeliveryOrderCreateDto(menuItem.Id, ingredient.Id);

        _mockMapper.Setup(m => m.Map<Order>(dto)).Returns(new Order { Id = Guid.NewGuid() });
        _mockMapper.Setup(m => m.Map<OrderReadDto>(It.IsAny<Order>()))
            .Returns(new OrderReadDto { Id = Guid.NewGuid(), OrderType = "Delivery" });
        _mockMapper.Setup(m => m.Map<DeliveryOrderCreatedEvent>(It.IsAny<Order>()))
            .Returns(new DeliveryOrderCreatedEvent());

        // Act
        var result = await _service.CreateDeliveryOrder(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.OrderType.Should().Be("Delivery");
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<DeliveryOrderCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateDeliveryOrder_ShouldFail_WhenMenuItemDoesNotExist()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient();
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        var invalidMenuItemId = Guid.NewGuid();
        var dto = OrderTestData.CreateDeliveryOrderCreateDto(invalidMenuItemId, ingredient.Id);

        // Act
        var result = await _service.CreateDeliveryOrder(dto);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Object reference not set to an instance of an object."
        );
    }

    [Fact]
    public async Task GetOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetOrder(nonExistentId);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.NotFound,
            "Order not found"
        );
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOrder_WhenExists()
    {
        // Arrange
        var menuItem = MenuItemTestData.CreateMenuItem();
        var order = OrderTestData.CreateOrder();
        var orderItem = OrderItemTestData.CreateOrderItem(orderId: order.Id, menuItemId: menuItem.Id);

        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.OrderItems.AddAsync(orderItem);
        await _dbContext.SaveChangesAsync();

        var expectedDto = new OrderReadDto { Id = order.Id };
        _mockMapper.Setup(m => m.Map<OrderReadDto>(It.Is<Order>(o => o.Id == order.Id)))
                   .Returns(expectedDto);

        // Act
        var result = await _service.GetOrder(order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnAllOrders_WhenStatusIsNull()
    {
        // Arrange
        var order1 = OrderTestData.CreateOrder(status: OrderStatus.Ongoing);
        var order2 = OrderTestData.CreateOrder(status: OrderStatus.Closed);
        await _dbContext.Orders.AddRangeAsync(order1, order2);
        await _dbContext.SaveChangesAsync();

        var expectedDtos = new List<OrderReadDto>
    {
        new() { Id = order1.Id },
        new() { Id = order2.Id }
    };

        _mockMapper.Setup(m => m.Map<List<OrderReadDto>>(It.IsAny<List<Order>>()))
                   .Returns(expectedDtos);

        // Act
        var result = await _service.GetOrders(null);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnFilteredOrders_WhenStatusProvided()
    {
        // Arrange
        var ongoingOrder = OrderTestData.CreateOrder(status: OrderStatus.Ongoing);
        var closedOrder = OrderTestData.CreateOrder(status: OrderStatus.Closed);

        await _dbContext.Orders.AddRangeAsync(ongoingOrder, closedOrder);
        await _dbContext.SaveChangesAsync();

        var expectedDtos = new List<OrderReadDto>
    {
        new() { Id = ongoingOrder.Id }
    };

        _mockMapper.Setup(m => m.Map<List<OrderReadDto>>(It.Is<List<Order>>(orders =>
            orders.All(o => o.Status == OrderStatus.Ongoing))))
            .Returns(expectedDtos);

        // Act
        var result = await _service.GetOrders(OrderStatus.Ongoing);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(1);
        result.Data.Should().BeEquivalentTo(expectedDtos);
    }

    [Fact]
    public async Task GetOrders_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        _mockMapper.Setup(m => m.Map<List<OrderReadDto>>(It.IsAny<List<Order>>()))
                   .Throws(new Exception("Unexpected failure"));

        // Act
        var result = await _service.GetOrders(null);

        // Assert
        result.ShouldFailWith<List<OrderReadDto>>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Unexpected failure"
        );
    }

    //[Fact]
    //public async Task GetOngoingNonDineInOrders_ShouldReturnBadRequest_WhenOrderTypeIsInvalid()
    //{
    //    // Arrange
    //    var invalidOrderType = OrderType.DineIn;
    //    var date = DateTime.Today;

    //    // Act
    //    var result = await _service.GetOngoingAndClosedNonDineInOrders(invalidOrderType, date);

    //    // Assert
    //    result.ShouldFailWith<List<NonDineInOrderSummaryDto>>(
    //        HttpStatusCode.BadRequest,
    //        "Invalid order type. Only 'Delivery' and 'Takeaway' types are allowed."
    //    );
    //}

    //[Fact]
    //public async Task GetOngoingNonDineInOrders_ShouldReturnOrders_WhenValid()
    //{
    //    // Arrange
    //    var date = DateTime.Today;
    //    var order = OrderTestData.CreateOrder(
    //        type: OrderType.Takeaway,
    //        status: OrderStatus.Ongoing
    //    );

    //    await _dbContext.Orders.AddAsync(order);
    //    await _dbContext.SaveChangesAsync();

    //    var expectedDto = new NonDineInOrderSummaryDto { Id = order.Id };
    //    _mockMapper.Setup(m => m.Map<List<NonDineInOrderSummaryDto>>(It.IsAny<List<Order>>()))
    //        .Returns(new List<NonDineInOrderSummaryDto> { expectedDto });

    //    // Act
    //    var result = await _service.GetOngoingAndClosedNonDineInOrders(OrderType.Takeaway, date);

    //    // Assert
    //    result.ShouldBeSuccessful(HttpStatusCode.OK);
    //    result.Data.Should().ContainSingle().And.ContainEquivalentOf(expectedDto);
    //}

    //[Fact]
    //public async Task GetOngoingNonDineInOrders_ShouldFail_WhenExceptionThrown()
    //{
    //    // Arrange
    //    var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);

    //    // Act
    //    var result = await service.GetOngoingAndClosedNonDineInOrders(OrderType.Delivery, date );

    //    // Assert
    //    result.ShouldFailWith<List<NonDineInOrderSummaryDto>>(
    //        HttpStatusCode.InternalServerError,
    //        expectedErrorMessage: result.ErrorMessage!
    //    );
    //}

    [Fact]
    public async Task GetOngoingOrdersForTable_ShouldReturnNotFound_WhenNoOngoingOrdersExist()
    {
        // Arrange
        var tableId = Guid.NewGuid();

        // Act
        var result = await _service.GetOngoingOrdersForTable(tableId);

        // Assert
        result.ShouldFailWith<List<OrderSummaryDto>>(
            HttpStatusCode.NotFound,
            "No ongoing orders found for this table."
        );
    }

    [Fact]
    public async Task GetOngoingOrdersForTable_ShouldReturnOrders_WhenOngoingOrdersExist()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var order = OrderTestData.CreateOrder(
            status: OrderStatus.Ongoing,
            tableId: tableId
        );
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var expectedDto = new OrderSummaryDto { Id = order.Id };
        _mockMapper.Setup(m => m.Map<List<OrderSummaryDto>>(It.IsAny<List<Order>>()))
            .Returns(new List<OrderSummaryDto> { expectedDto });

        // Act
        var result = await _service.GetOngoingOrdersForTable(tableId);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().ContainSingle().And.ContainEquivalentOf(expectedDto);
    }

    [Fact]
    public async Task GetOngoingOrdersForTable_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object); // simulate failure

        // Act
        var result = await service.GetOngoingOrdersForTable(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task ApplyOrderDiscount_ShouldFail_WhenOrderDoesNotExist()
    {
        // Arrange
        var nonExistentOrderId = Guid.NewGuid();

        // Act
        var result = await _service.ApplyOrderDiscount(nonExistentOrderId, 10);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.NotFound,
            "Order not found."
        );
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(150)]
    public async Task ApplyOrderDiscount_ShouldFail_WhenDiscountIsInvalid(decimal invalidDiscount)
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.ApplyOrderDiscount(order.Id, invalidDiscount);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "Invalid discount percentage."
        );
    }

    [Fact]
    public async Task ApplyOrderDiscount_ShouldSucceed_WhenValidDiscountApplied()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var mappedDto = new OrderReadDto { Id = order.Id, Discount = 10 };
        _mockMapper.Setup(m => m.Map<OrderReadDto>(order)).Returns(mappedDto);
        _mockMapper.Setup(m => m.Map<OrderDiscountAppliedEvent>(order)).Returns(new OrderDiscountAppliedEvent());

        // Act
        var result = await _service.ApplyOrderDiscount(order.Id, 10);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Discount.Should().Be(10);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderDiscountAppliedEvent>()), Times.Once);
    }

    [Fact]
    public async Task ApplyOrderDiscount_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.ApplyOrderDiscount(Guid.NewGuid(), 10);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.InternalServerError,
            expectedErrorMessage: result.ErrorMessage! // dynamic because exact exception text may vary
        );
    }

    [Fact]
    public async Task ChangeOrderTable_ShouldFail_WhenOrderNotFound()
    {
        // Act
        var result = await _service.ChangeOrderTable(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.NotFound, "Order not found.");
    }

    [Fact]
    public async Task ChangeOrderTable_ShouldFail_WhenOrderHasNoTable()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(tableId: null);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.ChangeOrderTable(order.Id, Guid.NewGuid());

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "This order is not associated with a table."
        );
    }

    [Fact]
    public async Task ChangeOrderTable_ShouldFail_WhenNewTableDoesNotExist()
    {
        // Arrange
        var table = TableTestData.CreateCorrectTable();
        var order = OrderTestData.CreateOrder(tableId: table.Id);
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.ChangeOrderTable(order.Id, Guid.NewGuid());

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "Specified table does not exist."
        );
    }

    [Fact]
    public async Task ChangeOrderTable_ShouldFail_WhenNewTableIsOngoing()
    {
        // Arrange
        var oldTable = TableTestData.CreateOngoingTable();
        var newTable = TableTestData.CreateOngoingTable();
        var order = OrderTestData.CreateOrder(tableId: oldTable.Id);

        await _dbContext.Tables.AddRangeAsync(oldTable, newTable);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.ChangeOrderTable(order.Id, newTable.Id);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.Conflict,
            "The specified table is currently occupied."
        );
    }

    [Fact]
    public async Task ChangeOrderTable_ShouldSucceed_WhenValid()
    {
        // Arrange
        var oldTable = TableTestData.CreateOngoingTable();
        var newTable = TableTestData.CreateCorrectTable();
        var order = OrderTestData.CreateOrder(tableId: oldTable.Id);

        await _dbContext.Tables.AddRangeAsync(oldTable, newTable);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<OrderReadDto>(order)).Returns(new OrderReadDto { Id = order.Id });
        _mockMapper.Setup(m => m.Map<OrderTableChangedEvent>(order)).Returns(new OrderTableChangedEvent());

        // Act
        var result = await _service.ChangeOrderTable(order.Id, newTable.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(order.Id);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderTableChangedEvent>()), Times.Once);

        var updatedOrder = await _dbContext.Orders.FindAsync(order.Id);
        updatedOrder!.TableId.Should().Be(newTable.Id);
    }

    [Fact]
    public async Task CloseOrder_ShouldFail_WhenOrderNotFound()
    {
        // Act
        var result = await _service.CloseOrder(Guid.NewGuid());

        // Assert
        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.NotFound, "Order not found.");
    }

    [Fact]
    public async Task CloseOrder_ShouldFail_WhenOrderAlreadyClosed()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(status: OrderStatus.Closed);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.CloseOrder(order.Id);

        // Assert
        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.BadRequest, "Order is already closed.");
    }

    [Fact]
    public async Task CloseOrder_ShouldFail_WhenPaymentsDoNotCoverTotal()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(totalAmount: 100, discount: 0);
        order.Payments.Add(PaymentTestData.CreatePayment(amount: 50));

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.CloseOrder(order.Id);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "Payments do not fully cover the order total."
        );
    }

    [Fact]
    public async Task CloseOrder_ShouldSucceed_WhenAllConditionsMet()
    {
        // Arrange
        var table = TableTestData.CreateOngoingTable();
        var order = OrderTestData.CreateOrder(tableId: table.Id, totalAmount: 100, discount: 0);
        var payment = PaymentTestData.CreatePayment(amount: 100);
        order.Payments.Add(payment);

        await _dbContext.Tables.AddAsync(table);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<OrderReadDto>(order)).Returns(new OrderReadDto { Id = order.Id });
        _mockMapper.Setup(m => m.Map<OrderClosedEvent>(order)).Returns(new OrderClosedEvent());

        // Act
        var result = await _service.CloseOrder(order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(order.Id);
        order.Status.Should().Be(OrderStatus.Closed);
        table.Status.Should().Be(TableStatus.Available);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderClosedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CloseOrder_ShouldNotFreeTable_WhenOtherOpenOrdersExist()
    {
        // Arrange
        var table = TableTestData.CreateOngoingTable();
        var order1 = OrderTestData.CreateOrder(tableId: table.Id, totalAmount: 100, discount: 0);
        var order2 = OrderTestData.CreateOrder(tableId: table.Id, status: OrderStatus.Ongoing);
        var payment = PaymentTestData.CreatePayment(amount: 100);
        order1.Payments.Add(payment);

        await _dbContext.Tables.AddAsync(table);
        await _dbContext.Orders.AddRangeAsync(order1, order2);
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<OrderReadDto>(order1)).Returns(new OrderReadDto { Id = order1.Id });
        _mockMapper.Setup(m => m.Map<OrderClosedEvent>(order1)).Returns(new OrderClosedEvent());

        // Act
        var result = await _service.CloseOrder(order1.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(order1.Id);
        table.Status.Should().Be(TableStatus.Ongoing); // Not reset to Available
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldSucceed_WhenOrderExists()
    {
        // Arrange
        var table = TableTestData.CreateCorrectTable();
        var order = OrderTestData.CreateOrder(tableId: table.Id, status: OrderStatus.Ongoing);

        await _dbContext.Tables.AddAsync(table);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        _mockMapper
            .Setup(m => m.Map<OrderStatusChangedEvent>(It.Is<(Order, OrderStatus)>(tuple =>
                tuple.Item1.Id == order.Id && tuple.Item2 == OrderStatus.Ongoing
            )))
            .Returns(new OrderStatusChangedEvent());

        _mockMapper
            .Setup(m => m.Map<OrderReadDto>(order))
            .Returns(new OrderReadDto { Id = order.Id, TableId = table.Id });

        // Act
        var result = await _service.UpdateOrderStatus(order.Id, OrderStatus.Closed);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(order.Id);
        result.Data.TableId.Should().Be(table.Id);

        var updatedOrder = await _dbContext.Orders.FindAsync(order.Id);
        updatedOrder!.Status.Should().Be(OrderStatus.Closed);

        var updatedTable = await _dbContext.Tables.FindAsync(table.Id);
        updatedTable!.Status.Should().Be(TableStatus.Available);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderStatusChangedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldFail_WhenOrderDoesNotExist()
    {
        // Act
        var result = await _service.UpdateOrderStatus(Guid.NewGuid(), OrderStatus.Closed);

        // Assert
        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.NotFound, "Order not found");
    }

    [Fact]
    public async Task UpdateOrderStatus_ShouldFail_WhenExceptionIsThrown()
    {
        // Arrange
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.UpdateOrderStatus(Guid.NewGuid(), OrderStatus.Closed);

        // Assert
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task UpdateOrderType_ShouldSucceed_WhenTypeChangesToDelivery()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(type: OrderType.Takeaway);
        var dto = new OrderUpdateTypeDto
        {
            NewOrderType = OrderType.Delivery,
            PhoneNumber = "123456789",
            Address = "Test Street 1"
        };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<OrderTypeChangeEvent>(
            It.Is<(Order, OrderType)>(tuple => tuple.Item1.Id == order.Id && tuple.Item2 == OrderType.Takeaway)
        )).Returns(new OrderTypeChangeEvent());

        _mockMapper.Setup(m => m.Map<OrderReadDto>(order)).Returns(new OrderReadDto { Id = order.Id });

        // Act
        var result = await _service.UpdateOrderType(order.Id, dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(order.Id);

        var updatedOrder = await _dbContext.Orders.FindAsync(order.Id);
        updatedOrder!.Type.Should().Be(OrderType.Delivery);
        updatedOrder.CustomerInformation!.Address.Should().Be("Test Street 1");

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderTypeChangeEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderType_ShouldFail_WhenOrderNotFound()
    {
        var dto = new OrderUpdateTypeDto { NewOrderType = OrderType.DineIn, TableId = Guid.NewGuid() };

        var result = await _service.UpdateOrderType(Guid.NewGuid(), dto);

        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.NotFound, "Order not found.");
    }

    [Fact]
    public async Task UpdateOrderType_ShouldFail_WhenTypeAlreadySet()
    {
        var order = OrderTestData.CreateOrder(type: OrderType.Takeaway);
        var dto = new OrderUpdateTypeDto { NewOrderType = OrderType.Takeaway };

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var result = await _service.UpdateOrderType(order.Id, dto);

        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.BadRequest, "Order type is already set to the requested type.");
    }

    [Fact]
    public async Task UpdateOrderType_ShouldFail_WhenInvalidType()
    {
        var order = OrderTestData.CreateOrder();
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var dto = new OrderUpdateTypeDto { NewOrderType = (OrderType)99 }; // invalid

        var result = await _service.UpdateOrderType(order.Id, dto);

        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.BadRequest, "Invalid order type.");
    }

    [Fact]
    public async Task UpdateOrderType_ShouldFail_WhenExceptionIsThrown()
    {
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);
        var dto = new OrderUpdateTypeDto { NewOrderType = OrderType.DineIn };

        var result = await service.UpdateOrderType(Guid.NewGuid(), dto);

        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

   

    [Fact]
    public async Task SplitOrder_ShouldFail_WhenOrderNotFound()
    {
        var dto = new MoveOrderItemsDto { SplitGroups = [] };

        var result = await _service.SplitOrder(Guid.NewGuid(), dto);

        result.ShouldFailWith<OrderReadDto>(HttpStatusCode.NotFound, "Order not found.");
    }

    [Fact]
    public async Task SplitOrder_ShouldFail_WhenOrderHasPayments()
    {
        var order = OrderTestData.CreateOrder();
        order.Payments.Add(PaymentTestData.CreatePayment(amount: 10));

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var dto = new MoveOrderItemsDto { SplitGroups = [] };

        var result = await _service.SplitOrder(order.Id, dto);

        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "Order cannot be split after payments have been made."
        );
    }

    [Fact]
    public async Task SplitOrder_ShouldSucceed_WhenNoItemsMatchGroups()
    {
        var order = OrderTestData.CreateOrder();
        var orderItem = OrderItemTestData.CreateOrderItem(order.Id, Guid.NewGuid(), price: 12);
        order.OrderItems.Add(orderItem);

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var dto = new MoveOrderItemsDto
        {
            SplitGroups = new List<SplitOrderGroupDto>
        {
            new SplitOrderGroupDto { OrderItemIds = new List<Guid> { Guid.NewGuid() } } // no match
        }
        };

        var result = await _service.SplitOrder(order.Id, dto);

        result.ShouldBeSuccessful(HttpStatusCode.Created);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderSplitEvent>()), Times.Never);
    }

    [Fact]
    public async Task SplitOrder_ShouldFail_WhenExceptionThrown()
    {
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);
        var dto = new MoveOrderItemsDto { SplitGroups = [] };

        var result = await service.SplitOrder(Guid.NewGuid(), dto);

        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task JoinOrder_ShouldSucceed_WhenValidOrders()
    {
        // Arrange
        var sourceOrder = OrderTestData.CreateOrder();
        var targetOrder = OrderTestData.CreateOrder();

        var menuItem = MenuItemTestData.CreateMenuItem();
        var sourceItem = OrderItemTestData.CreateOrderItem(sourceOrder.Id, menuItem.Id, price: 10);
        var targetItem = OrderItemTestData.CreateOrderItem(targetOrder.Id, menuItem.Id, price: 5);

        sourceOrder.OrderItems.Add(sourceItem);
        targetOrder.OrderItems.Add(targetItem);

        await _dbContext.Orders.AddRangeAsync(sourceOrder, targetOrder);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<OrderJoinEvent>(It.Is<Order>(o => o.Id == targetOrder.Id)))
            .Returns(new OrderJoinEvent());

        // Act
        var result = await _service.JoinOrder(sourceOrder.Id, targetOrder.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeNull();
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<OrderJoinEvent>()), Times.Once);

        var remainingOrder = await _dbContext.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == targetOrder.Id);
        remainingOrder!.OrderItems.Should().HaveCount(2); // both items combined

        var deletedOrder = await _dbContext.Orders.FindAsync(sourceOrder.Id);
        deletedOrder.Should().BeNull();
    }

    [Fact]
    public async Task JoinOrder_ShouldFail_WhenOneOrBothOrdersNotFound()
    {
        // Act
        var result = await _service.JoinOrder(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.NotFound,
            "One or both orders not found."
        );
    }

    [Fact]
    public async Task JoinOrder_ShouldFail_WhenOrdersHavePayments()
    {
        // Arrange
        var sourceOrder = OrderTestData.CreateOrder();
        var targetOrder = OrderTestData.CreateOrder();

        sourceOrder.Payments.Add(PaymentTestData.CreatePayment(amount: 10));

        await _dbContext.Orders.AddRangeAsync(sourceOrder, targetOrder);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.JoinOrder(sourceOrder.Id, targetOrder.Id);

        // Assert
        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "Orders cannot be joined after payments have been made."
        );
    }

    [Fact]
    public async Task JoinOrder_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.JoinOrder(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task MoveOrderItems_ShouldFail_WhenOrdersNotFound()
    {
        var dto = new SplitOrderGroupDto { OrderItemIds = new List<Guid>() };

        var result = await _service.MoveOrderItems(Guid.NewGuid(), Guid.NewGuid(), dto);

        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.NotFound,
            "One or both orders not found."
        );
    }

    [Fact]
    public async Task MoveOrderItems_ShouldFail_WhenPaymentsExist()
    {
        var sourceOrder = OrderTestData.CreateOrder();
        var targetOrder = OrderTestData.CreateOrder();
        sourceOrder.Payments.Add(PaymentTestData.CreatePayment(amount: 20));

        await _dbContext.Orders.AddRangeAsync(sourceOrder, targetOrder);
        await _dbContext.SaveChangesAsync();

        var dto = new SplitOrderGroupDto { OrderItemIds = new List<Guid>() };

        var result = await _service.MoveOrderItems(sourceOrder.Id, targetOrder.Id, dto);

        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "Cannot move items after payments have been made."
        );
    }

    [Fact]
    public async Task MoveOrderItems_ShouldFail_WhenNoValidItemsSelected()
    {
        var menuItem = MenuItemTestData.CreateMenuItem();
        var sourceOrder = OrderTestData.CreateOrder();
        var targetOrder = OrderTestData.CreateOrder();

        var item = OrderItemTestData.CreateOrderItem(sourceOrder.Id, menuItem.Id);
        sourceOrder.OrderItems.Add(item);

        await _dbContext.Orders.AddRangeAsync(sourceOrder, targetOrder);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SaveChangesAsync();

        var dto = new SplitOrderGroupDto { OrderItemIds = new List<Guid> { Guid.NewGuid() } }; // Invalid ID

        var result = await _service.MoveOrderItems(sourceOrder.Id, targetOrder.Id, dto);

        result.ShouldFailWith<OrderReadDto>(
            HttpStatusCode.BadRequest,
            "No valid items selected for moving."
        );
    }

    [Fact]
    public async Task MoveOrderItems_ShouldFail_WhenExceptionThrown()
    {
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);

        var result = await service.MoveOrderItems(Guid.NewGuid(), Guid.NewGuid(), new SplitOrderGroupDto());

        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task DeleteOrder_ShouldSucceed_WhenOrderExists()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var menuItem = MenuItemTestData.CreateMenuItem();
        var orderItem = OrderItemTestData.CreateOrderItem(order.Id, menuItem.Id);
        order.OrderItems.Add(orderItem);

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.DeleteOrder(order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeTrue();

        var deletedOrder = await _dbContext.Orders.FindAsync(order.Id);
        deletedOrder.Should().BeNull();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.Is<OrderDeletedEvent>(ev => ev.OrderId == order.Id)), Times.Once);
    }

    [Fact]
    public async Task DeleteOrder_ShouldFail_WhenOrderNotFound()
    {
        // Act
        var result = await _service.DeleteOrder(Guid.NewGuid());

        // Assert
        result.ShouldFailWith(
            HttpStatusCode.NotFound,
            "order not found"
        );
    }

    [Fact]
    public async Task DeleteOrder_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var service = new OrderService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await service.DeleteOrder(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }


}
