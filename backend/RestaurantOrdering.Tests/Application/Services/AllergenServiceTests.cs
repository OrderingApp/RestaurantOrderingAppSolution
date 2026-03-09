using Application.Dtos.Allergens;
using Application.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Allergens;
using RestaurantOrdering.Tests.TestData;
using System.Net;

namespace RestaurantOrdering.Tests.Application.Services;

public class AllergenServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AllergenService _service;

    public AllergenServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new AllergenService(
            _dbContext,
            _mockEventHandler.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task CreateAllergen_ShouldSucceed_WhenValidAllergen()
    {
        // Arrange
        var allergen = AllergenTestData.CreateValidAllergen(euNumber: AllergenTestData.DefaultEuNumber);
        var dto = new AllergenCreateDto { Name = allergen.Name, EuNumber = allergen.EuNumber };
        var readDto = new AllergenReadDto { Id = allergen.Id, Name = allergen.Name, EuNumber = allergen.EuNumber };

        _mockMapper.Setup(m => m.Map<Allergen>(dto)).Returns(allergen);
        _mockMapper.Setup(m => m.Map<AllergenReadDto>(allergen)).Returns(readDto);
        _mockMapper
            .Setup(m => m.Map<AllergenCreatedEvent>(allergen))
            .Returns(new AllergenCreatedEvent());

        // Act
        var result = await _service.CreateAllergen(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(allergen.Name);
        result.Data.EuNumber.Should().Be(AllergenTestData.DefaultEuNumber);
        _dbContext.Allergens.Should().ContainSingle(a => a.Name == allergen.Name);
        _mockEventHandler.Verify(
            e => e.HandleEventAsync(It.IsAny<AllergenCreatedEvent>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAllergen_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var dto = new AllergenCreateDto { Name = "Invalid" };

        _mockMapper
            .Setup(m => m.Map<Allergen>(It.IsAny<AllergenCreateDto>()))
            .Throws(new Exception("Database failure"));

        // Act
        var result = await _service.CreateAllergen(dto);

        // Assert
        result.ShouldFailWith<AllergenReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Database failure"
        );
    }

    [Fact]
    public async Task GetAllergen_ShouldSucceed_WhenAllergenExists()
    {
        // Arrange
        var allergen = AllergenTestData.CreateValidAllergen();
        await _dbContext.Allergens.AddAsync(allergen);
        await _dbContext.SaveChangesAsync();

        var readDto = new AllergenReadDto { Id = allergen.Id, Name = allergen.Name };
        _mockMapper.Setup(m => m.Map<AllergenReadDto>(allergen)).Returns(readDto);

        // Act
        var result = await _service.GetAllergen(allergen.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(allergen.Id);
        result.Data.Name.Should().Be(allergen.Name);
    }

    [Fact]
    public async Task GetAllergen_ShouldFail_WhenAllergenDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetAllergen(nonExistentId);

        // Assert
        result.ShouldFailWith<AllergenReadDto>(HttpStatusCode.NotFound, "Allergen not found.");
    }

    [Fact]
    public async Task UpdateAllergen_ShouldSucceed_WhenAllergenExists()
    {
        // Arrange
        var allergen = AllergenTestData.CreateValidAllergen();
        await _dbContext.Allergens.AddAsync(allergen);
        await _dbContext.SaveChangesAsync();

        var updateDto = new AllergenUpdateDto
        {
            Name = AllergenTestData.UpdatedAllergenName,
            IsUsed = true,
        };
        var updatedDto = new AllergenReadDto
        {
            Id = allergen.Id,
            Name = AllergenTestData.UpdatedAllergenName,
        };

        _mockMapper
            .Setup(m => m.Map(updateDto, allergen))
            .Callback(() => allergen.Name = updateDto.Name!);
        _mockMapper.Setup(m => m.Map<AllergenReadDto>(allergen)).Returns(updatedDto);
        _mockMapper
            .Setup(m => m.Map<AllergenUpdatedEvent>(allergen))
            .Returns(new AllergenUpdatedEvent());

        // Act
        var result = await _service.UpdateAllergen(updateDto, allergen.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(allergen.Id);
        result.Data.Name.Should().Be(AllergenTestData.UpdatedAllergenName);
        _mockEventHandler.Verify(
            e => e.HandleEventAsync(It.IsAny<AllergenUpdatedEvent>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAllergen_ShouldFail_WhenAllergenDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new AllergenUpdateDto { Name = "DoesNotMatter", IsUsed = true };

        // Act
        var result = await _service.UpdateAllergen(updateDto, nonExistentId);

        // Assert
        result.ShouldFailWith<AllergenReadDto>(HttpStatusCode.NotFound, "Allergen not found.");
    }

    [Fact]
    public async Task DeleteAllergen_ShouldSucceed_WhenAllergenExists()
    {
        // Arrange
        var allergen = AllergenTestData.CreateValidAllergen();
        await _dbContext.Allergens.AddAsync(allergen);
        await _dbContext.SaveChangesAsync();

        _mockMapper
            .Setup(m => m.Map<AllergenDeletedEvent>(allergen))
            .Returns(new AllergenDeletedEvent());

        // Act
        var result = await _service.DeleteAllergen(allergen.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeTrue();

        var softDeleted = await _dbContext.Allergens.FindAsync(allergen.Id);
        softDeleted!.IsDeleted.Should().BeTrue();
        softDeleted.IsUsed.Should().BeFalse();

        _mockEventHandler.Verify(
            e => e.HandleEventAsync(It.IsAny<AllergenDeletedEvent>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAllergen_ShouldFail_WhenAllergenDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.DeleteAllergen(nonExistentId);

        // Assert
        result.ShouldFailWith<bool>(HttpStatusCode.NotFound, "Allergen not found.");
    }

    [Fact]
    public async Task DeleteAllergen_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var allergen = AllergenTestData.CreateValidAllergen();
        await _dbContext.Allergens.AddAsync(allergen);
        await _dbContext.SaveChangesAsync();

        _mockEventHandler
            .Setup(e => e.HandleEventAsync(It.IsAny<AllergenDeletedEvent>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.DeleteAllergen(allergen.Id);

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Database error"
        );
    }

    [Fact]
    public async Task CreateAllergen_ShouldFail_WhenEuNumberAlreadyTaken()
    {
        // Arrange — persist an allergen with EU number 1 directly in the DB
        var existing = AllergenTestData.CreateValidAllergen(euNumber: 1);
        await _dbContext.Allergens.AddAsync(existing);
        await _dbContext.SaveChangesAsync();

        var dto = new AllergenCreateDto { Name = "Another Gluten", EuNumber = 1 };

        // Act
        var result = await _service.CreateAllergen(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
        result.ErrorMessage.Should().Contain("EU allergen number 1 is already assigned");
    }

    [Fact]
    public async Task UpdateAllergen_ShouldFail_WhenEuNumberAlreadyTakenByAnotherAllergen()
    {
        // Arrange — two allergens, try to assign the first one's EU number to the second
        var allergen1 = AllergenTestData.CreateValidAllergen(euNumber: 1);
        var allergen2 = AllergenTestData.CreateValidAllergen(euNumber: null);
        await _dbContext.Allergens.AddRangeAsync(allergen1, allergen2);
        await _dbContext.SaveChangesAsync();

        var updateDto = new AllergenUpdateDto
        {
            Name = allergen2.Name,
            EuNumber = 1,   // same as allergen1
            IsUsed = true,
        };

        // Act
        var result = await _service.UpdateAllergen(updateDto, allergen2.Id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
        result.ErrorMessage.Should().Contain("EU allergen number 1 is already assigned");
    }

    [Fact]
    public async Task UpdateAllergen_ShouldSucceed_WhenKeepingOwnEuNumber()
    {
        // Arrange — updating an allergen while keeping its own EU number must not conflict
        var allergen = AllergenTestData.CreateValidAllergen(euNumber: 1);
        await _dbContext.Allergens.AddAsync(allergen);
        await _dbContext.SaveChangesAsync();

        var updateDto = new AllergenUpdateDto
        {
            Name = "Updated Gluten",
            EuNumber = 1,   // same number, same allergen — no conflict
            IsUsed = true,
        };

        var updatedDto = new AllergenReadDto { Id = allergen.Id, Name = "Updated Gluten", EuNumber = 1 };
        _mockMapper.Setup(m => m.Map(updateDto, allergen)).Callback(() => allergen.Name = updateDto.Name);
        _mockMapper.Setup(m => m.Map<AllergenReadDto>(allergen)).Returns(updatedDto);
        _mockMapper.Setup(m => m.Map<AllergenUpdatedEvent>(allergen)).Returns(new AllergenUpdatedEvent());

        // Act
        var result = await _service.UpdateAllergen(updateDto, allergen.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.EuNumber.Should().Be(1);
    }
}
