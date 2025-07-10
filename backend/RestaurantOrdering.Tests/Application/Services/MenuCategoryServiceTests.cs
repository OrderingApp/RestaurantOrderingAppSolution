using Application.Dtos.MenuCategories;
using Application.Dtos.MenuItems;
using Application.Dtos.SubCategories;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.MenuCategories;
using System.Net;



namespace RestaurantOrdering.Tests.Application.Services;

public class MenuCategoryServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly MenuCategoryService _service;

    public MenuCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockMapper = new Mock<IMapper>();
        _mockEventHandler = new Mock<IEventHandlerService>();
        _service = new MenuCategoryService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateMenuCategory_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var createDto = new MenuCategoryCreateDto { Name = "Pizza" };
        var entity = new MenuCategory { Id = Guid.NewGuid(), Name = "Pizza" };
        var readDto = new MenuCategoryReadDto { Id = entity.Id, Name = "Pizza" };

        _mockMapper.Setup(m => m.Map<MenuCategory>(createDto)).Returns(entity);
        _mockMapper.Setup(m => m.Map<MenuCategoryReadDto>(entity)).Returns(readDto);
        _mockMapper.Setup(m => m.Map<MenuCategoryCreatedEvent>(entity)).Returns(new MenuCategoryCreatedEvent());

        // Act
        var result = await _service.CreateMenuCategory(createDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be("Pizza");

        var saved = await _dbContext.MenuCategories.FindAsync(entity.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Pizza");

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuCategoryCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateMenuCategory_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var createDto = new MenuCategoryCreateDto { Name = "Exploding" };

        _mockMapper.Setup(m => m.Map<MenuCategory>(createDto))
            .Throws(new Exception("Mapping exploded"));

        // Act
        var result = await _service.CreateMenuCategory(createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred: Mapping exploded");
    }

    [Fact]
    public async Task GetMenuCategory_ShouldReturnMenuCategory_WhenItExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new MenuCategory { Id = id, Name = "Drinks" };
        var readDto = new MenuCategoryReadDto { Id = id, Name = "Drinks" };

        _dbContext.MenuCategories.Add(entity);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<MenuCategoryReadDto>(entity)).Returns(readDto);

        // Act
        var result = await _service.GetMenuCategory(id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(id);
        result.Data.Name.Should().Be("Drinks");
    }

    [Fact]
    public async Task GetMenuCategory_ShouldReturnNotFound_WhenItDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetMenuCategory(nonExistentId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
        result.ErrorMessage.Should().Be("MenuCategory not found.");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMenuCategory_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var badService = new MenuCategoryService(
            null!, // null DbContext to simulate failure
            _mockEventHandler.Object,
            _mockMapper.Object
        );

        // Act
        var result = await badService.GetMenuCategory(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
        result.Data.Should().BeNull();
    }


    [Fact]
    public async Task GetMenuCategories_ShouldReturnValidResults_WhenCategoriesExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var subCategoryId = Guid.NewGuid();

        var menuCategory = new MenuCategory
        {
            Id = categoryId,
            Name = "Main",
            IsUsed = true,
            IsDeleted = false,
            SequenceNumber = 1,
            SubCategories = new List<SubCategory>
        {
            new SubCategory { Id = subCategoryId, Name = "Subs", SequenceNumber = 1 }
        }
        };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Burger",
            IsUsed = true,
            IsDeleted = false,
            MenuCategoryId = categoryId,
            SubCategoryId = subCategoryId
        };

        await _dbContext.MenuCategories.AddAsync(menuCategory);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<List<MenuCategoryReadDto>>(It.IsAny<List<MenuCategory>>()))
            .Returns(new List<MenuCategoryReadDto>
            {
            new MenuCategoryReadDto
            {
                Id = categoryId,
                Name = "Main",
                TotalItems = 1,
                SubCategories = new List<SubCategoryReadDto>
                {
                    new SubCategoryReadDto { Id = subCategoryId, Name = "Subs", TotalItems = 1 }
                }
            }
            });

        // Act
        var result = await _service.GetMenuCategories();

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("Main");
        result.Data.First().TotalItems.Should().Be(1);
        result.Data.First().SubCategories.Should().ContainSingle();
        result.Data.First().SubCategories.First().TotalItems.Should().Be(1);
    }

    [Fact]
    public async Task GetMenuCategories_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        // Arrange
        _mockMapper.Setup(m => m.Map<List<MenuCategoryReadDto>>(It.IsAny<List<MenuCategory>>()))
            .Returns(new List<MenuCategoryReadDto>());

        // Act
        var result = await _service.GetMenuCategories();

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().NotBeNull();
        result.Data!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMenuCategories_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var badService = new MenuCategoryService(
            null!, // force exception
            _mockEventHandler.Object,
            _mockMapper.Object
        );

        // Act
        var result = await badService.GetMenuCategories();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task GetMenuCategoriesWithHierarchy_ShouldReturnFilteredResults_WhenFilteringApplied()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var subCategoryId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();

        var tag = new Tag { Id = tagId, Name = "Vegan" };
        var ingredient = new Ingredient
        {
            Id = ingredientId,
            Name = "test",
            IngredientTagRels = new List<IngredientTagRel> { new() { TagId = tagId, Tag = tag } }
        };
        var menuItem = new MenuItem
        {
            Id = menuItemId,
            Name = "Salad",
            IsUsed = true,
            IsDeleted = false,
            SubCategoryId = subCategoryId,
            MenuItemIngredientRels = new List<MenuItemIngredientRel>
        {
            new() { Ingredient = ingredient, IngredientId = ingredientId }
        }
        };
        var subCategory = new SubCategory { Id = subCategoryId, Name = "Cold", SequenceNumber = 1 };
        var category = new MenuCategory
        {
            Id = categoryId,
            Name = "Starters",
            IsUsed = true,
            IsDeleted = false,
            SequenceNumber = 1,
            SubCategories = new List<SubCategory> { subCategory },
            MenuItems = new List<MenuItem> { menuItem }
        };

        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.MenuCategories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var expectedDto = new MenuCategoryHierarchyReadDto
        {
            Id = categoryId,
            Name = "Starters",
            SubCategories = new(),
            MenuItems = new List<MenuItemReadDto>
        {
            new() { Id = menuItemId, Name = "Salad" }
        }
        };

        _mockMapper.Setup(m => m.Map<List<MenuCategoryHierarchyReadDto>>(It.IsAny<List<MenuCategory>>()))
            .Returns(new List<MenuCategoryHierarchyReadDto> { expectedDto });

        var request = new GetMenuCategoryHierarchyRequest
        {
            MenuCategoryId = categoryId,
            SubCategoryId = subCategoryId,
            TagIds = new List<Guid> { tagId }
        };

        // Act
        var result = await _service.GetMenuCategoriesWithHierarchy(request);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle();
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetMenuCategoriesWithHierarchy_ShouldReturnEmpty_WhenNoMatches()
    {
        // Arrange
        var request = new GetMenuCategoryHierarchyRequest
        {
            MenuCategoryId = Guid.NewGuid(),
            TagIds = new List<Guid> { Guid.NewGuid() }
        };

        _mockMapper.Setup(m => m.Map<List<MenuCategoryHierarchyReadDto>>(It.IsAny<List<MenuCategory>>()))
            .Returns(new List<MenuCategoryHierarchyReadDto>());

        // Act
        var result = await _service.GetMenuCategoriesWithHierarchy(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetMenuCategoriesWithHierarchy_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var request = new GetMenuCategoryHierarchyRequest();
        var badService = new MenuCategoryService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await badService.GetMenuCategoriesWithHierarchy(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.Data.Should().BeEmpty();
        result.ErrorMessage.Should().Contain("An error occurred:");
    }


}
