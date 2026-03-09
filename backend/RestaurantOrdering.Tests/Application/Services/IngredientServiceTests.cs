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
using RestaurantOrdering.Tests.TestData;
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
        var dto = IngredientTestData.CreateCreateDto();
        var ingredient = IngredientTestData.CreateValidIngredient(name: dto.Name, price: dto.Price);
        var readDto = IngredientTestData.CreateReadDto(ingredient.Id, ingredient.Name, ingredient.Price);

        _mockMapper.Setup(m => m.Map<Ingredient>(dto)).Returns(ingredient);
        _mockMapper.Setup(m => m.Map<IngredientReadDto>(ingredient)).Returns(readDto);
        _mockMapper.Setup(m => m.Map<IngredientCreatedEvent>(It.IsAny<Ingredient>()))
            .Returns(new IngredientCreatedEvent());

        // Act
        var result = await _service.CreateIngredient(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(dto.Name);

        var saved = await _dbContext.Ingredients.FirstOrDefaultAsync(i => i.Name == dto.Name);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateIngredient_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var dto = IngredientTestData.CreateCreateDto(name: "Exploding Ingredient", price: 5);

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
        var ingredients = IngredientTestData.CreateDefaultIngredients();
        _dbContext.Ingredients.AddRange(ingredients);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<List<IngredientReadDto>>(It.IsAny<List<Ingredient>>()))
            .Returns<List<Ingredient>>(ing =>
                ing.Select(i => new IngredientReadDto
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
        result.Data!.Select(i => i.Name).Should().Contain(new[] {
        IngredientTestData.CheeseName,
        IngredientTestData.TomatoName
    });
        result.Data!.Select(i => i.Name).Should().NotContain(IngredientTestData.BaconName);
    }

    [Fact]
    public async Task GetIngredients_ShouldFilterByTags_WhenTagsAreProvided()
    {
        // Arrange
        var veganTag = TagTestData.CreateTag(TagTestData.VeganTag);
        var spicyTag = TagTestData.CreateTag(TagTestData.SpicyTag);

        var cheese = IngredientTestData.CreateTaggedIngredient(IngredientTestData.CheeseName, 3m, veganTag);
        var tomato = IngredientTestData.CreateTaggedIngredient(IngredientTestData.TomatoName, 1.5m, spicyTag);
        var bacon = IngredientTestData.CreateUntaggedIngredient(IngredientTestData.BaconName, 2m);

        _dbContext.Tags.AddRange(veganTag, spicyTag);
        _dbContext.Ingredients.AddRange(tomato, bacon, cheese);
        await _dbContext.SaveChangesAsync();

        var inputTags = new List<string> { TagTestData.VeganTag };

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
        result.Data!.First().Name.Should().Be(IngredientTestData.CheeseName);
    }

    [Fact]
    public async Task GetIngredients_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var tags = new List<string> { TagTestData.VeganTag };

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
        var ingredient = IngredientTestData.CreateValidIngredient();
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(ingredient))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Price = ingredient.Price
            });

        var result = await _service.GetIngredient(ingredient.Id);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be(ingredient.Name);
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
        var ingredient = IngredientTestData.CreateValidIngredient(name: IngredientTestData.TomatoName);
        var tag1 = TagTestData.CreateTag(TagTestData.VeganTag);
        var tag2 = TagTestData.CreateTag(TagTestData.SpicyTag);

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.Tags.AddRangeAsync(tag1, tag2);
        await _dbContext.SaveChangesAsync();

        var tagIds = new List<Guid> { tag1.Id, tag2.Id };

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Tags = new List<string> { TagTestData.VeganTag, TagTestData.SpicyTag }
            });

        // Act
        var result = await _service.AddTagsToIngredient(ingredient.Id, tagIds);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Tags.Should().Contain(new[] { TagTestData.VeganTag, TagTestData.SpicyTag });

        var updatedIngredient = await _dbContext.Ingredients
            .Include(i => i.IngredientTagRels)
            .ThenInclude(r => r.Tag)
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id);

        updatedIngredient!.IngredientTagRels.Select(r => r.Tag.Name)
            .Should().Contain(new[] { TagTestData.VeganTag, TagTestData.SpicyTag });
    }

    [Fact]
    public async Task AddTagsToIngredient_ShouldNotAddDuplicateTags()
    {
        // Arrange
        var tag = TagTestData.CreateTag(TagTestData.SpicyTag);
        var ingredient = IngredientTestData.CreateValidIngredient(name: IngredientTestData.TomatoName);
        var rel = new IngredientTagRel { IngredientId = ingredient.Id, TagId = tag.Id, Tag = tag };

        ingredient.IngredientTagRels.Add(rel);

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        var tagIds = new List<Guid> { tag.Id };

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Tags = new List<string> { TagTestData.SpicyTag }
            });

        // Act
        var result = await _service.AddTagsToIngredient(ingredient.Id, tagIds);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Tags.Should().ContainSingle(t => t == TagTestData.SpicyTag);

        var updatedIngredient = await _dbContext.Ingredients
            .Include(i => i.IngredientTagRels)
            .ThenInclude(r => r.Tag)
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id);

        updatedIngredient!.IngredientTagRels.Should().HaveCount(1);
        updatedIngredient.IngredientTagRels.First().TagId.Should().Be(tag.Id);
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
        var ingredient = IngredientTestData.CreateValidIngredient(name: "Old Name", price: 1.5m);
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        var updateDto = IngredientTestData.CreateUpdateDto();
        var expectedReadDto = IngredientTestData.CreateReadDto(ingredient.Id, updateDto.Name!, updateDto.Price!.Value);

        _mockMapper.Setup(m => m.Map(updateDto, ingredient))
            .Callback<IngredientUpdateDto, Ingredient>((src, dest) =>
            {
                dest.Name = src.Name!;
                dest.Price = src.Price!.Value;
            });

        _mockMapper.Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(expectedReadDto);

        _mockMapper.Setup(m => m.Map<IngredientUpdatedEvent>(It.IsAny<Ingredient>()))
            .Returns(new IngredientUpdatedEvent { IngredientId = ingredient.Id });

        // Act
        var result = await _service.UpdateIngredient(ingredient.Id, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be(IngredientTestData.UpdatedIngredientName);

        var saved = await _dbContext.Ingredients.FindAsync(ingredient.Id);
        saved!.Name.Should().Be(IngredientTestData.UpdatedIngredientName);
        saved.Price.Should().Be(updateDto.Price);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<IngredientUpdatedEvent>()), Times.Once);
    }


    [Fact]
    public async Task UpdateIngredient_ShouldReturnNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = IngredientTestData.CreateUpdateDto("Non-existent Ingredient", 4.0m);

        // Act
        var result = await _service.UpdateIngredient(nonExistentId, updateDto);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Ingredient not found.");
    }


    [Fact]
    public async Task DeleteIngredient_ShouldSoftDelete_WhenIngredientExists()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient(
            name: "Parsley",
            price: 0.50m
        );

        ingredient.MenuItemIngredientRels.Add(new MenuItemIngredientRel
        {
            IngredientId = ingredient.Id,
            MenuItemId = Guid.NewGuid()
        });

        ingredient.IngredientTagRels.Add(new IngredientTagRel
        {
            IngredientId = ingredient.Id,
            TagId = Guid.NewGuid()
        });

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

    [Fact]
    public async Task AddAllergensToIngredient_ShouldSucceed_WhenNewAllergensAreAdded()
    {
        // Arrange
        var ingredient = IngredientTestData.CreateValidIngredient(name: IngredientTestData.TomatoName);
        var allergen1 = AllergenTestData.CreateAllergen(AllergenTestData.GlutenName);
        var allergen2 = AllergenTestData.CreateAllergen(AllergenTestData.NutsName);

        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.Allergens.AddRangeAsync(allergen1, allergen2);
        await _dbContext.SaveChangesAsync();

        var allergenIds = new List<Guid> { allergen1.Id, allergen2.Id };

        _mockMapper
            .Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Allergens = new List<string>
                {
                    AllergenTestData.GlutenName,
                    AllergenTestData.NutsName,
                },
            });

        // Act
        var result = await _service.AddAllergensToIngredient(ingredient.Id, allergenIds);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Allergens.Should()
            .Contain(new[] { AllergenTestData.GlutenName, AllergenTestData.NutsName });

        var updatedIngredient = await _dbContext
            .Ingredients.Include(i => i.IngredientAllergenRels)
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id);
        updatedIngredient!.IngredientAllergenRels.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAllergensToIngredient_ShouldNotAddDuplicateAllergens()
    {
        // Arrange
        var allergen = AllergenTestData.CreateAllergen(AllergenTestData.GlutenName);
        var ingredient = IngredientTestData.CreateValidIngredient();
        ingredient.IngredientAllergenRels.Add(
            new IngredientAllergenRel { IngredientId = ingredient.Id, AllergenId = allergen.Id, Allergen = allergen }
        );

        await _dbContext.Allergens.AddAsync(allergen);
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.SaveChangesAsync();

        _mockMapper
            .Setup(m => m.Map<IngredientReadDto>(It.IsAny<Ingredient>()))
            .Returns(new IngredientReadDto
            {
                Id = ingredient.Id,
                Name = ingredient.Name,
                Allergens = new List<string> { AllergenTestData.GlutenName },
            });

        // Act - try to add the same allergen again
        var result = await _service.AddAllergensToIngredient(
            ingredient.Id,
            new List<Guid> { allergen.Id }
        );

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);

        var updatedIngredient = await _dbContext
            .Ingredients.Include(i => i.IngredientAllergenRels)
            .FirstOrDefaultAsync(i => i.Id == ingredient.Id);
        updatedIngredient!.IngredientAllergenRels.Should().HaveCount(1);
    }

    [Fact]
    public async Task AddAllergensToIngredient_ShouldReturnNotFound_WhenIngredientDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var allergenIds = new List<Guid> { Guid.NewGuid() };

        // Act
        var result = await _service.AddAllergensToIngredient(nonExistentId, allergenIds);

        // Assert
        result.ShouldFailWith<IngredientReadDto>(HttpStatusCode.NotFound, "Ingredient not found.");
    }
}
