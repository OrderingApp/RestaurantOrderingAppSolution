using Application.Dtos.Payments;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Payments;
using RestaurantOrdering.Tests.TestData;
using System.Net;

namespace RestaurantOrdering.Tests.Application.Services;

public class PaymentServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new PaymentService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task AddPayment_ShouldSucceed_WhenOrderIsPendingPayment()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(status: OrderStatus.PendingPayment, totalAmount: 100m);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var paymentDto = PaymentTestData.CreatePaymentCreateDto(50m, PaymentMethod.Card);
        var payment = PaymentTestData.CreatePayment(orderId: order.Id, amount: 50m, paymentMethod: PaymentMethod.Card);

        _mockMapper.Setup(m => m.Map<Payment>(paymentDto)).Returns(payment);
        _mockMapper.Setup(m => m.Map<PaymentReadDto>(payment))
            .Returns(new PaymentReadDto { Id = payment.Id, Amount = payment.Amount });
        _mockMapper.Setup(m => m.Map<PaymentCreatedEvent>(payment))
            .Returns(new PaymentCreatedEvent());

        // Act
        var result = await _service.AddPayment(order.Id, paymentDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Amount.Should().Be(50m);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<PaymentCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task AddPayment_ShouldFail_WhenOrderDoesNotExist()
    {
        // Arrange
        var paymentDto = PaymentTestData.CreatePaymentCreateDto(50m);

        // Act
        var result = await _service.AddPayment(Guid.NewGuid(), paymentDto);

        // Assert
        result.ShouldFailWith<PaymentReadDto>(
            HttpStatusCode.NotFound,
            "Order not found"
        );
    }

    [Fact]
    public async Task AddPayment_ShouldFail_WhenOrderIsNotPendingPayment()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(status: OrderStatus.Ongoing, totalAmount: 100m);
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        var paymentDto = PaymentTestData.CreatePaymentCreateDto(50m);

        // Act
        var result = await _service.AddPayment(order.Id, paymentDto);

        // Assert
        result.ShouldFailWith<PaymentReadDto>(
            HttpStatusCode.BadRequest,
            "Order must be in Pending Payment status to add payment."
        );
    }

    [Fact]
    public async Task AddPayment_ShouldFail_WhenPaymentExceedsTotal()
    {
        // Arrange
        var order = OrderTestData.CreateOrder(status: OrderStatus.PendingPayment, totalAmount: 100m);
        var existingPayment = PaymentTestData.CreatePayment(orderId: order.Id, amount: 90m);
        order.Payments.Add(existingPayment);

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.Payments.AddAsync(existingPayment);
        await _dbContext.SaveChangesAsync();

        var paymentDto = PaymentTestData.CreatePaymentCreateDto(20m); // would exceed total

        // Act
        var result = await _service.AddPayment(order.Id, paymentDto);

        // Assert
        result.ShouldFailWith<PaymentReadDto>(
            HttpStatusCode.BadRequest,
            "Payment exceeds order total."
        );
    }


    [Fact]
    public async Task GetAllOrderPayments_ShouldSucceed_WhenPaymentsExist()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var payment1 = PaymentTestData.CreatePayment(orderId: order.Id, amount: 50m);
        var payment2 = PaymentTestData.CreatePayment(orderId: order.Id, amount: 25m);

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.Payments.AddRangeAsync(payment1, payment2);
        await _dbContext.SaveChangesAsync();

        var paymentDtos = new List<PaymentReadDto>
    {
        new() { Id = payment1.Id, Amount = payment1.Amount },
        new() { Id = payment2.Id, Amount = payment2.Amount }
    };

        _mockMapper.Setup(m => m.Map<List<PaymentReadDto>>(It.IsAny<List<Payment>>()))
            .Returns(paymentDtos);

        // Act
        var result = await _service.GetAllOrderPayments(order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllOrderPayments_ShouldFail_WhenOrderDoesNotExist()
    {
        // Act
        var result = await _service.GetAllOrderPayments(Guid.NewGuid());

        // Assert
        result.ShouldFailWith<List<PaymentReadDto>>(
            HttpStatusCode.NotFound,
            "Order not found"
        );
    }

    [Fact]
    public async Task MarkPaymentAsRefunded_ShouldSucceed_WhenPaymentExists()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        var payment = PaymentTestData.CreatePayment(orderId: order.Id);

        await _dbContext.Orders.AddAsync(order);
        await _dbContext.Payments.AddAsync(payment);
        await _dbContext.SaveChangesAsync();

        var paymentReadDto = new PaymentReadDto { Id = payment.Id, Amount = payment.Amount, IsRefunded = true };
        _mockMapper.Setup(m => m.Map<PaymentReadDto>(payment)).Returns(paymentReadDto);

        // Act
        var result = await _service.MarkPaymentAsRefunded(payment.Id, order.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.IsRefunded.Should().BeTrue();

        var updatedPayment = await _dbContext.Payments.FindAsync(payment.Id);
        updatedPayment!.IsRefunded.Should().BeTrue();
    }

    [Fact]
    public async Task MarkPaymentAsRefunded_ShouldFail_WhenOrderDoesNotExist()
    {
        // Act
        var result = await _service.MarkPaymentAsRefunded(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.ShouldFailWith<PaymentReadDto>(
            HttpStatusCode.NotFound,
            "Order not found"
        );
    }

    [Fact]
    public async Task MarkPaymentAsRefunded_ShouldFail_WhenPaymentDoesNotExist()
    {
        // Arrange
        var order = OrderTestData.CreateOrder();
        await _dbContext.Orders.AddAsync(order);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.MarkPaymentAsRefunded(Guid.NewGuid(), order.Id);

        // Assert
        result.ShouldFailWith<PaymentReadDto>(
            HttpStatusCode.NotFound,
            "Payment not found"
        );
    }
}
