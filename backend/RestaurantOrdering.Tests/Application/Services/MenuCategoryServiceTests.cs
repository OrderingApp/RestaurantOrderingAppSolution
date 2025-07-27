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
using RestaurantOrdering.Tests.TestData;
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
        var createDto = MenuCategoryTestData.CreateCreateDto();
        var entity = MenuCategoryTestData.CreateValidCategory(name: createDto.Name);
        var readDto = MenuCategoryTestData.CreateReadDto(entity.Id, entity.Name);

        _mockMapper.Setup(m => m.Map<MenuCategory>(createDto)).Returns(entity);
        _mockMapper.Setup(m => m.Map<MenuCategoryReadDto>(entity)).Returns(readDto);
        _mockMapper.Setup(m => m.Map<MenuCategoryCreatedEvent>(entity)).Returns(new MenuCategoryCreatedEvent());

        // Act
        var result = await _service.CreateMenuCategory(createDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(createDto.Name);

        var saved = await _dbContext.MenuCategories.FindAsync(entity.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be(createDto.Name);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuCategoryCreatedEvent>()), Times.Once);
    }


    [Fact]
    public async Task CreateMenuCategory_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var createDto = MenuCategoryTestData.CreateCreateDto("Exploding");

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
        var entity = MenuCategoryTestData.CreateValidCategory(id, "Drinks");
        var readDto = MenuCategoryTestData.CreateReadDto(id, "Drinks");

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
        var menuItemId = Guid.NewGuid();

        var subCategory = SubCategoryTestData.CreateValidSubCategory(
            id: subCategoryId,
            name: "Subs",
            sequence: 1
        );

        var menuCategory = MenuCategoryTestData.CreateValidCategory(
            id: categoryId,
            name: "Main"
        );
        menuCategory.SubCategories.Add(subCategory);

        var menuItem = MenuItemTestData.CreateMenuItem(menuItemId);
        menuItem.Name = "Burger";
        menuItem.IsUsed = true;
        menuItem.IsDeleted = false;
        menuItem.MenuCategoryId = categoryId;
        menuItem.SubCategoryId = subCategoryId;

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
        var tag = TagTestData.CreateTag("Vegan");
        var ingredient = IngredientTestData.CreateTaggedIngredient("test", 1m, tag);
        var subCategory = SubCategoryTestData.CreateSubCategory("Cold");
        var menuItem = MenuItemTestData.CreateMenuItem(name: "Salad");
        var menuCategory = MenuCategoryTestData.CreateValidCategory(name: "Starters");

        // Link relationships
        menuItem.MenuItemIngredientRels = [new() { Ingredient = ingredient, IngredientId = ingredient.Id }];
        menuItem.SubCategoryId = subCategory.Id;
        menuCategory.MenuItems.Add(menuItem);
        menuCategory.SubCategories.Add(subCategory);

        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.Ingredients.AddAsync(ingredient);
        await _dbContext.MenuItems.AddAsync(menuItem);
        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.MenuCategories.AddAsync(menuCategory);
        await _dbContext.SaveChangesAsync();

        var expectedDto = new MenuCategoryHierarchyReadDto
        {
            Id = menuCategory.Id,
            Name = menuCategory.Name,
            SubCategories = new(),
            MenuItems = new List<MenuItemReadDto>
        {
            new() { Id = menuItem.Id, Name = "Salad" }
        }
        };

        _mockMapper.Setup(m => m.Map<List<MenuCategoryHierarchyReadDto>>(It.IsAny<List<MenuCategory>>()))
            .Returns(new List<MenuCategoryHierarchyReadDto> { expectedDto });

        var request = new GetMenuCategoryHierarchyRequest
        {
            MenuCategoryId = menuCategory.Id,
            SubCategoryId = subCategory.Id,
            TagIds = new List<Guid> { tag.Id }
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

    [Fact]
    public async Task UpdateMenuCategory_ShouldSucceed_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var entity = MenuCategoryTestData.CreateValidCategory(id: categoryId, name: "Old Name");

        await _dbContext.MenuCategories.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        var updateDto = MenuCategoryTestData.CreateUpdateDto(name: MenuCategoryTestData.UpdatedCategoryName);

        _mockMapper.Setup(m => m.Map(updateDto, entity))
            .Callback<MenuCategoryUpdateDto, MenuCategory>((src, dest) =>
            {
                dest.Name = src.Name!;
            });

        var updatedDto = MenuCategoryTestData.CreateReadDto(categoryId, MenuCategoryTestData.UpdatedCategoryName);

        _mockMapper.Setup(m => m.Map<MenuCategoryReadDto>(entity)).Returns(updatedDto);
        _mockMapper.Setup(m => m.Map<MenuCategoryUpdatedEvent>(entity))
            .Returns(new MenuCategoryUpdatedEvent());

        // Act
        var result = await _service.UpdateMenuCategory(categoryId, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be(MenuCategoryTestData.UpdatedCategoryName);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuCategoryUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateMenuCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = MenuCategoryTestData.CreateUpdateDto("Updated Name");

        // Act
        var result = await _service.UpdateMenuCategory(nonExistentId, updateDto);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "MenuCategory not found or has been deleted.");
    }

    [Fact]
    public async Task UpdateMenuCategory_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = MenuCategoryTestData.CreateUpdateDto("Test");

        // Simulate exception
        var mockService = new MenuCategoryService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await mockService.UpdateMenuCategory(id, updateDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }

    [Fact]
    public async Task DeleteMenuCategory_ShouldSoftDelete_WhenCategoryExistsAndNotDeleted()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = MenuCategoryTestData.CreateValidCategory(
            id: categoryId,
            name: "Starters",
            isDeleted: false,
            isUsed: true
        );

        _dbContext.MenuCategories.Add(category);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<MenuCategoryDeletedEvent>(category))
            .Returns(new MenuCategoryDeletedEvent());

        // Act
        var result = await _service.DeleteMenuCategory(categoryId);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);

        var deleted = await _dbContext.MenuCategories.FindAsync(categoryId);
        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
        deleted.IsUsed.Should().BeFalse();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<MenuCategoryDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMenuCategory_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.DeleteMenuCategory(nonExistentId);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "MenuCategory not found.");
    }

    [Fact]
    public async Task DeleteMenuCategory_ShouldReturnBadRequest_WhenCategoryAlreadyDeleted()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var alreadyDeleted = MenuCategoryTestData.CreateValidCategory(
            id: categoryId,
            name: "Archived",
            isDeleted: true
        );

        _dbContext.MenuCategories.Add(alreadyDeleted);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.DeleteMenuCategory(categoryId);

        // Assert
        result.ShouldFailWith(HttpStatusCode.BadRequest, "MenuCategory has already been deleted.");
    }

    [Fact]
    public async Task DeleteMenuCategory_ShouldReturnError_WhenExceptionThrown()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        var mockService = new MenuCategoryService(null!, _mockEventHandler.Object, _mockMapper.Object);

        // Act
        var result = await mockService.DeleteMenuCategory(categoryId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.InternalServerError);
        result.ErrorMessage.Should().Contain("An error occurred:");
    }
}
