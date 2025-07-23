using Application.Dtos.SubCategories;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.SubCategories;
using RestaurantOrdering.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrdering.Tests.Application.Services;

public class SubCategoryServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly SubCategoryService _service;

    public SubCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new SubCategoryService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateSubCategory_ShouldSucceed_WhenValidMenuCategoryExists()
    {
        // Arrange
        var menuCategory = MenuCategoryTestData.CreateValidCategory();
        await _dbContext.MenuCategories.AddAsync(menuCategory);
        await _dbContext.SaveChangesAsync();

        var createDto = new SubCategoryCreateDto
        {
            Name = SubCategoryTestData.DefaultSubCategoryName,
            MenuCategoryId = menuCategory.Id
        };

        var subCategory = SubCategoryTestData.CreateValidSubCategory(menuCategory.Id);
        var subCategoryReadDto = new SubCategoryReadDto
        {
            Id = subCategory.Id,
            Name = SubCategoryTestData.DefaultSubCategoryName
        };

        _mockMapper.Setup(m => m.Map<SubCategory>(createDto)).Returns(subCategory);
        _mockMapper.Setup(m => m.Map<SubCategoryReadDto>(subCategory)).Returns(subCategoryReadDto);
        _mockMapper.Setup(m => m.Map<SubCategoryCreatedEvent>(subCategory)).Returns(new SubCategoryCreatedEvent());

        // Act
        var result = await _service.CreateSubCategory(createDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(SubCategoryTestData.DefaultSubCategoryName);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<SubCategoryCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateSubCategory_ShouldFail_WhenMenuCategoryDoesNotExist()
    {
        // Arrange
        var createDto = new SubCategoryCreateDto
        {
            Name = "Subs",
            MenuCategoryId = Guid.NewGuid() // invalid MenuCategoryId
        };

        // Act
        var result = await _service.CreateSubCategory(createDto);

        // Assert
        result.ShouldFailWith<SubCategoryReadDto>(
            HttpStatusCode.BadRequest,
            "Invalid MenuCategoryId. The referenced menu category does not exist."
        );
    }

    [Fact]
    public async Task GetSubCategory_ShouldSucceed_WhenSubCategoryExists()
    {
        // Arrange
        var subCategory = SubCategoryTestData.CreateValidSubCategory();
        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.SaveChangesAsync();

        var subCategoryDto = new SubCategoryReadDto
        {
            Id = subCategory.Id,
            Name = subCategory.Name
        };

        _mockMapper.Setup(m => m.Map<SubCategoryReadDto>(subCategory)).Returns(subCategoryDto);

        // Act
        var result = await _service.GetSubCategory(subCategory.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(subCategory.Id);
        result.Data.Name.Should().Be(subCategory.Name);
    }

    [Fact]
    public async Task GetSubCategory_ShouldFail_WhenSubCategoryDoesNotExist()
    {
        // Act
        var result = await _service.GetSubCategory(Guid.NewGuid());

        // Assert
        result.ShouldFailWith<SubCategoryReadDto>(
            HttpStatusCode.NotFound,
            "SubCategory not found."
        );
    }

    [Fact]
    public async Task UpdateSubCategory_ShouldFail_WhenSubCategoryDoesNotExist()
    {
        // Arrange
        var updateDto = new SubCategoryUpdateDto
        {
            Name = "Updated Name",
            MenuCategoryId = Guid.NewGuid()
        };

        // Act
        var result = await _service.UpdateSubCategory(Guid.NewGuid(), updateDto);

        // Assert
        result.ShouldFailWith<SubCategoryReadDto>(
            HttpStatusCode.NotFound,
            "subCategory not found or has been deleted."
        );
    }

    [Fact]
    public async Task UpdateSubCategory_ShouldFail_WhenMenuCategoryDoesNotExist()
    {
        // Arrange
        var subCategory = SubCategoryTestData.CreateValidSubCategory();
        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.SaveChangesAsync();

        var updateDto = new SubCategoryUpdateDto
        {
            Name = "Updated Name",
            MenuCategoryId = Guid.NewGuid() // nonexistent MenuCategory
        };

        // Act
        var result = await _service.UpdateSubCategory(subCategory.Id, updateDto);

        // Assert
        result.ShouldFailWith<SubCategoryReadDto>(
            HttpStatusCode.BadRequest,
            "Invalid MenuCategoryId. The referenced menu category does not exist."
        );
    }

    [Fact]
    public async Task UpdateSubCategory_ShouldSucceed_WhenValid()
    {
        // Arrange
        var menuCategory = MenuCategoryTestData.CreateValidCategory();
        var subCategory = SubCategoryTestData.CreateValidSubCategory();
        await _dbContext.MenuCategories.AddAsync(menuCategory);
        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.SaveChangesAsync();

        var updateDto = new SubCategoryUpdateDto
        {
            Name = SubCategoryTestData.UpdatedSubCategoryName,
            MenuCategoryId = menuCategory.Id
        };

        var mappedDto = new SubCategoryReadDto
        {
            Id = subCategory.Id,
            Name = SubCategoryTestData.UpdatedSubCategoryName
        };

        _mockMapper.Setup(m => m.Map(updateDto, subCategory)).Verifiable();
        _mockMapper.Setup(m => m.Map<SubCategoryReadDto>(subCategory)).Returns(mappedDto);
        _mockMapper.Setup(m => m.Map<SubCategoryUpdatedEvent>(subCategory))
            .Returns(new SubCategoryUpdatedEvent());

        // Act
        var result = await _service.UpdateSubCategory(subCategory.Id, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be(SubCategoryTestData.UpdatedSubCategoryName);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<SubCategoryUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSubCategory_ShouldFail_WhenSubCategoryDoesNotExist()
    {
        // Act
        var result = await _service.DeleteSubCategory(Guid.NewGuid());

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.NotFound,
            "subCategory not found."
        );
    }

    [Fact]
    public async Task DeleteSubCategory_ShouldFail_WhenSubCategoryAlreadyDeleted()
    {
        // Arrange
        var subCategory = SubCategoryTestData.CreateValidSubCategory();
        subCategory.IsDeleted = true;

        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.DeleteSubCategory(subCategory.Id);

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.BadRequest,
            "subCategory has already been deleted."
        );
    }

    [Fact]
    public async Task DeleteSubCategory_ShouldSucceed_WhenValid()
    {
        // Arrange
        var subCategory = SubCategoryTestData.CreateValidSubCategory();

        await _dbContext.SubCategories.AddAsync(subCategory);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<SubCategoryDeletedEvent>(subCategory))
            .Returns(new SubCategoryDeletedEvent());

        // Act
        var result = await _service.DeleteSubCategory(subCategory.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeTrue();

        var deletedSubCategory = await _dbContext.SubCategories.FindAsync(subCategory.Id);
        deletedSubCategory!.IsDeleted.Should().BeTrue();
        deletedSubCategory.IsUsed.Should().BeFalse();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<SubCategoryDeletedEvent>()), Times.Once);
    }

}
