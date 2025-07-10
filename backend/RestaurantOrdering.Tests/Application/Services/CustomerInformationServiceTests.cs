using System.Net;
using Application.Dtos.Common;
using Application.Dtos.CustomerInformations;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.CustomerInformations;

public class CustomerInformationServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly CustomerInformationService _service;

    public CustomerInformationServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockMapper = new Mock<IMapper>();
        _mockEventHandler = new Mock<IEventHandlerService>();
        _service = new CustomerInformationService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task UpdateCustomerInformation_ShouldFail_WhenCustomerNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new CustomerInformationUpdateDto { PhoneNumber = "123456789" };

        var result = await _service.UpdateCustomerInformation(id, dto);

        result.ShouldFailWith(HttpStatusCode.NotFound, "Customer information not found.");
    }

    [Fact]
    public async Task UpdateCustomerInformation_ShouldSucceed_WhenDataIsValid()
    {
        var customer = new CustomerInformation
        {
            Id = Guid.NewGuid(),
            PhoneNumber = "111",
            OrderCompletionType = OrderCompletionType.Immediate,
            PreferredPaymentMethod = PreferredPaymentMethod.Cash
        };
        _dbContext.CustomerInformation.Add(customer);
        await _dbContext.SaveChangesAsync();

        var dto = new CustomerInformationUpdateDto
        {
            PhoneNumber = "999",
            OrderCompletionType = OrderCompletionType.Scheduled,
            PreferedPaymentMethod = PreferredPaymentMethod.Card
        };

        _mockMapper.Setup(m => m.Map(dto, customer))
            .Callback<CustomerInformationUpdateDto, CustomerInformation>((src, dest) =>
            {
                dest.PhoneNumber = src.PhoneNumber;
                dest.OrderCompletionType = src.OrderCompletionType;
                dest.PreferredPaymentMethod = src.PreferedPaymentMethod;
            });

        _mockMapper.Setup(m => m.Map<CustomerInformationReadDto>(It.IsAny<CustomerInformation>()))
            .Returns(new CustomerInformationReadDto { PhoneNumber = dto.PhoneNumber });

        _mockMapper.Setup(m => m.Map<CustomerInformationUpdatedEvent>(It.IsAny<CustomerInformation>()))
            .Returns(new CustomerInformationUpdatedEvent { CustomerId = customer.Id } );

        var result = await _service.UpdateCustomerInformation(customer.Id, dto);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.PhoneNumber.Should().Be("999");

        var saved = await _dbContext.CustomerInformation.FindAsync(customer.Id);
        saved!.PhoneNumber.Should().Be("999");

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<CustomerInformationUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task GetCustomerInformation_ShouldFail_WhenNotFound()
    {
        var result = await _service.GetCustomerInformation(Guid.NewGuid());

        result.ShouldFailWith(HttpStatusCode.NotFound, "Customer information not found.");
    }

    [Fact]
    public async Task GetCustomerInformation_ShouldSucceed_WhenFound()
    {
        var customer = new CustomerInformation
        {
            Id = Guid.NewGuid(),
            PhoneNumber = "555",
        };

        _dbContext.CustomerInformation.Add(customer);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<CustomerInformationReadDto>(customer))
            .Returns(new CustomerInformationReadDto { PhoneNumber = customer.PhoneNumber });

        var result = await _service.GetCustomerInformation(customer.Id);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.PhoneNumber.Should().Be("555");
    }
}
