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
        result.Data!.Name.Should().Be(dto.Name);

        var saved = await _dbContext.Ingredients.FirstOrDefaultAsync(a => a.Name == dto.Name);
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

    [Fact]
    public async Task GetIngredients_ShouldFilterByTags_WhenTagsAreProvided()
    {
        // Arrange
        var tag1 = new Tag { Id = Guid.NewGuid(), Name = "vegan" };
        var tag2 = new Tag { Id = Guid.NewGuid(), Name = "spicy" };

        var ingredient1 = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Tofu",
            Price = 3,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            IngredientTagRels = new List<IngredientTagRel>
        {
            new IngredientTagRel { Tag = tag1, TagId = tag1.Id }
        }
        };

        var ingredient2 = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Chili",
            Price = 1.5m,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            IngredientTagRels = new List<IngredientTagRel>
        {
            new IngredientTagRel { Tag = tag2, TagId = tag2.Id }
        }
        };

        var ingredient3 = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Cheese",
            Price = 2,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            IngredientTagRels = new List<IngredientTagRel>() // No tags
        };

        _dbContext.Tags.AddRange(tag1, tag2);
        _dbContext.Ingredients.AddRange(ingredient1, ingredient2, ingredient3);
        await _dbContext.SaveChangesAsync();

        var inputTags = new List<string> { "vegan" };

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
        var result = await _service.GetIngredients(inputTags);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("Tofu");
    }

    [Fact]
    public async Task GetIngredients_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var tags = new List<string> { "vegan" };

        // Force an exception by mocking the DbContext if desired, 
        // Another valid approach is to pass `null` to simulate bad setup.

        var badService = new IngredientService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await badService.GetIngredients(tags);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetIngredient_ShouldReturnData_WhenIngredientExists()
    {
        var ingredient = new Ingredient { Id = Guid.NewGuid(), Name = "Olive", Price = 1.5m };
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(ingredient))
            .Returns(new IngredientReadDto { Id = ingredient.Id, Name = "Olive", Price = 1.5m });

        var result = await _service.GetIngredient(ingredient.Id);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be("Olive");
    }

    [Fact]
    public async Task GetIngredient_ShouldReturnNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetIngredient(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        result.ErrorMessage.Should().Be("Ingredient not found.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task AddTagsToIngredient_ShouldSucceed_WhenNewTagsAreAdded()
    {
        // Arrange
        var ingredientId = Guid.NewGuid();
        var tag1 = new Tag { Id = Guid.NewGuid(), Name = "Vegan" };
        var tag2 = new Tag { Id = Guid.NewGuid(), Name = "Spicy" };

        var ingredient = new Ingredient
        {
            Id = ingredientId,
            Name = "Tomato",
            IngredientTagRels = new List<IngredientTagRel>()
        };

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.Tags.AddRangeAsync(tag1, tag2);
        await _dbContext.SaveChangesAsync();

        var tagIds = new List<Guid> { tag1.Id, tag2.Id };

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Tags = new List<string> { "Vegan", "Spicy" }
            });

        // Act
        var result = await _service.AddTagsToIngredient(ingredientId, tagIds);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Tags.Should().Contain(new[] { "Vegan", "Spicy" });

        var updatedIngredient = await _dbContext.Ingredients
            .Include(i => i.IngredientTagRels)
            .ThenInclude(r => r.Tag)
            .FirstOrDefaultAsync(i => i.Id == ingredientId);

        updatedIngredient!.IngredientTagRels.Select(r => r.Tag.Name).Should().Contain(new[] { "Vegan", "Spicy" });
    }


    [Fact]
    public async Task AddTagsToIngredient_ShouldNotAddDuplicateTags()
    {
        // Arrange
        var ingredientId = Guid.NewGuid();
        var tagId = Guid.NewGuid();

        var existingTag = new Tag { Id = tagId, Name = "Spicy" };

        var ingredient = new Ingredient
        {
            Id = ingredientId,
            Name = "Tomato",
            IngredientTagRels = new List<IngredientTagRel>
        {
            new IngredientTagRel { IngredientId = ingredientId, TagId = tagId, Tag = existingTag }
        }
        };

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.Tags.AddAsync(existingTag);
        await _dbContext.SaveChangesAsync();

        var tagIds = new List<Guid> { tagId }; // Try to add the same tag again

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Tags = new List<string> { "Spicy" }
            });

        // Act
        var result = await _service.AddTagsToIngredient(ingredientId, tagIds);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Tags.Should().ContainSingle(t => t == "Spicy");

        var updatedIngredient = await _dbContext.Ingredients
            .Include(i => i.IngredientTagRels)
            .ThenInclude(r => r.Tag)
            .FirstOrDefaultAsync(i => i.Id == ingredientId);

        updatedIngredient!.IngredientTagRels.Should().HaveCount(1);
        updatedIngredient.IngredientTagRels.First().TagId.Should().Be(tagId);
    }

    [Fact]
    public async Task AddTagsToIngredient_ShouldReturnNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var tagIds = new List<Guid> { Guid.NewGuid() };

        // Act
        var result = await _service.AddTagsToIngredient(nonExistentId, tagIds);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Ingredient not found.");
    }

    [Fact]
    public async Task UpdateIngredient_ShouldSucceed_WhenIngredientExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ingredient = new Ingredient
        {
            Id = id,
            Name = "Old Name",
            Price = 1.5m,
            CanBeUsedAsExtra = true
        };
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();

        var updateDto = new IngredientUpdateDto
        {
            Name = "Updated Name",
            Price = 3.0m
        };

        _mockMapper.Setup(m => m.Map(updateDto, ingredient))
            .Callback<IngredientUpdateDto, Ingredient>((src, dest) =>
            {
                dest.Name = src.Name!;
                dest.Price = src.Price!.Value;
            });

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto { Id = id, Name = "Updated Name", Price = 3.0m });

        _mockMapper.Setup(m => m.Map<IngredientUpdatedEvent>(It.IsAny<Ingredient>()))
            .Returns(new IngredientUpdatedEvent { IngredientId = id });

        // Act
        var result = await _service.UpdateIngredient(id, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be("Updated Name");

        var saved = await _dbContext.Ingredients.FindAsync(id);
        saved!.Name.Should().Be("Updated Name");
        saved.Price.Should().Be(3.0m);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<IngredientUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateIngredient_ShouldReturnNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new IngredientUpdateDto
        {
            Name = "Non-existent Ingredient",
            Price = 4.0m
        };

        // Act
        var result = await _service.UpdateIngredient(nonExistentId, updateDto);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Ingredient not found.");
    }

    [Fact]
    public async Task DeleteIngredient_ShouldSoftDelete_WhenIngredientExists()
    {
        // Arrange
        var ingredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            Name = "Parsley",
            Price = 0.50m,
            CanBeUsedAsExtra = true,
            IsDeleted = false,
            MenuItemIngredientRels = new List<MenuItemIngredientRel>
        {
            new MenuItemIngredientRel { IngredientId = Guid.NewGuid(), MenuItemId = Guid.NewGuid() }
        },
            IngredientTagRels = new List<IngredientTagRel>
        {
            new IngredientTagRel { IngredientId = Guid.NewGuid(), TagId = Guid.NewGuid() }
        }
        };

        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.DeleteIngredient(ingredient.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.NoContent);

        var deleted = await _dbContext.Ingredients
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id);

        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
        deleted.CanBeUsedAsExtra.Should().BeFalse();

        var relatedMenuItemRels = await _dbContext.MenuItemIngredientRels
            .Where(r => r.IngredientId == ingredient.Id)
            .ToListAsync();
        relatedMenuItemRels.Should().BeEmpty();

        var relatedTagRels = await _dbContext.IngredientTagRels
            .Where(r => r.IngredientId == ingredient.Id)
            .ToListAsync();
        relatedTagRels.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteIngredient_ShouldReturnNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.DeleteIngredient(nonExistentId);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Ingredient not found.");
    }
}
