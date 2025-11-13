using Application.Dtos.MenuItems;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.MenuItems;
using RestaurantOrdering.Tests.TestData;
using System.Net;

namespace RestaurantOrdering.Tests.Application.Services;

public class MenuItemServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly MenuItemService _service;

    public MenuItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockMapper = new Mock<IMapper>();
        _mockEventHandler = new Mock<IEventHandlerService>();
        _service = new MenuItemService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateMenuItem_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient();
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        var createDto = MenuItemTestData.CreateCreateDto(ingredientIds: new() { ingredient.Id });
        var menuItem = MenuItemTestData.CreateMenuItem(name: createDto.Name, price: createDto.Price);
        var readDto = MenuItemTestData.CreateReadDto(menuItem, new List<MenuItemIngredientWithTagsDto>
            {
                new() { Id = ingredient.Id, Name = ingredient.Name }
            });

        _mockMapper.Setup(m => m.Map<MenuItem>(createDto)).Returns(menuItem);
        _mockMapper.Setup(m => m.Map<MenuItemReadDto>(menuItem)).Returns(readDto);
        _mockMapper.Setup(m => m.Map<MenuItemCreatedEvent>(menuItem)).Returns(new MenuItemCreatedEvent());

        // Act
        var result = await _service.CreateMenuItem(createDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(menuItem.Name);
        result.Data.Ingredients.Should().ContainSingle(i => i.Id == ingredient.Id);

        var saved = await _dbContext.MenuItems
            .Include(m => m.MenuItemIngredientRels)
            .FirstOrDefaultAsync(m => m.Id == menuItem.Id);

        saved.Should().NotBeNull();
        saved!.MenuItemIngredientRels.Should().ContainSingle(rel => rel.IngredientId == ingredient.Id);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuItemCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateMenuItem_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var createDto = MenuItemTestData.CreateCreateDto();

        _mockMapper
            .Setup(m => m.Map<MenuItem>(It.IsAny<MenuItemCreateDto>()))
            .Throws(new Exception("Something went wrong"));

        // Act
        var result = await _service.CreateMenuItem(createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred: Something went wrong");
    }
    [Fact]
    public async Task UpdateMenuItem_ShouldSucceed_WhenItemExists()
    {
        // Arrange
        var menuItem = MenuItemTestData.CreateMenuItem();
        var updateDto = new MenuItemUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 15.0m,
            IngredientIds = new List<Guid>()
        };

        _dbContext.MenuItems.Add(menuItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map(updateDto, menuItem))
            .Callback<MenuItemUpdateDto, MenuItem>((src, dest) =>
            {
                dest.Name = src.Name!;
                dest.Description = src.Description!;
                dest.Price = src.Price!.Value;
            });

        _mockMapper
            .Setup(m => m.Map<MenuItemReadDto>(It.Is<MenuItem>(mi => mi.Id == menuItem.Id)))
            .Returns((MenuItem mi) => MenuItemTestData.CreateReadDto(mi));

        _mockMapper
            .Setup(m => m.Map<MenuItemUpdatedEvent>(It.Is<MenuItem>(mi => mi.Id == menuItem.Id)))
            .Returns(new MenuItemUpdatedEvent());

        // Act
        var result = await _service.UpdateMenuItem(menuItem.Id, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be("Updated Name");
        result.Data.Description.Should().Be("Updated Description");
        result.Data.Price.Should().Be(15.0m);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuItemUpdatedEvent>()), Times.Once);
    }


    [Fact]
    public async Task UpdateMenuItem_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new MenuItemUpdateDto
        {
            Name = "Crash",
            Price = 10
        };

        var brokenService = new MenuItemService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await brokenService.UpdateMenuItem(id, updateDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task DeleteMenuItem_ShouldSoftDelete_WhenItemExists()
    {
        // Arrange
        var menuItem = MenuItemTestData.CreateMenuItem();
        _dbContext.MenuItems.Add(menuItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<MenuItemDeletedEvent>(menuItem)).Returns(new MenuItemDeletedEvent());

        // Act
        var result = await _service.DeleteMenuItem(menuItem.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);

        var deletedItem = await _dbContext.MenuItems.FindAsync(menuItem.Id);
        deletedItem.Should().NotBeNull();
        deletedItem!.IsDeleted.Should().BeTrue();
        deletedItem.IsUsed.Should().BeFalse();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuItemDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMenuItem_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        var brokenService = new MenuItemService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await brokenService.DeleteMenuItem(id);

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.InternalServerError,
            result.ErrorMessage!
        );
    }

}
