using Application.Dtos.Ingredients;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Ingredients;
using System.Net;

namespace RestaurantOrdering.Tests.Application.Services;

public class IngredientServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly IngredientService _service;

    public IngredientServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockMapper = new Mock<IMapper>();
        _mockEventHandler = new Mock<IEventHandlerService>();
        _service = new IngredientService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateIngredient_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var dto = new IngredientCreateDto { Name = "New Ingredient", Price = 2 };
        var ingredient = new Ingredient { Id = Guid.NewGuid(), Name = dto.Name, Price = dto.Price };
        
        _mockMapper.Setup(m => m.Map<Ingredient>(dto)).Returns(ingredient);
        _mockMapper.Setup(m => m.Map<IngredientReadDto>(ingredient)).Returns(new IngredientReadDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Price = ingredient.Price
        });

        _mockMapper.Setup(m => m.Map<IngredientCreatedEvent>(It.IsAny<Ingredient>()))
            .Returns(new IngredientCreatedEvent());

        // Act
        var result = await _service.CreateIngredient(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be("New Ingredient");

        var saved = await _dbContext.Ingredients.FirstOrDefaultAsync(a => a.Name == "New Ingredient");
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var dto = new IngredientCreateDto { Name = "Exploding Ingredient", Price = 5 };

        // Simulate a mapping exception
        _mockMapper.Setup(m => m.Map<Ingredient>(dto))
            .Throws(new Exception("Something went wrong during mapping"));

        // Act
        var result = await _service.CreateIngredient(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occured: Something went wrong during mapping");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetIngredients_ShouldReturnAllIngredients_WhenNoTagsGiven()
    {
        // Arrange
        var ingredient1 = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Cheese",
            Price = 1.5m,
            CanBeUsedAsExtra = true,
            IsDeleted = false
        };

        var ingredient2 = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Tomato",
            Price = 1m,
            CanBeUsedAsExtra = true,
            IsDeleted = false
        };

        var ingredient3 = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Bacon",
            Price = 2m,
            CanBeUsedAsExtra = false, // Should be excluded
            IsDeleted = false
        };

        _dbContext.Ingredients.AddRange(ingredient1, ingredient2, ingredient3);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<List<IngredientReadDto>>(It.IsAny<List<Ingredient>>()))
            .Returns<List<Ingredient>>(ingredients =>
                ingredients.Select(i => new IngredientReadDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price
                }).ToList()
            );

        // Act
        var result = await _service.GetIngredients();

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
        result.Data!.Select(i => i.Name).Should().Contain(new[] { "Cheese", "Tomato" });
        result.Data!.Select(i => i.Name).Should().NotContain("Bacon");
    }

}
