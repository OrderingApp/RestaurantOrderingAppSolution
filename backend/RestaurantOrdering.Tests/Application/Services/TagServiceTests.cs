using Application.Dtos.Tags;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Tags;
using RestaurantOrdering.Tests.TestData;
using System.Net;

namespace RestaurantOrdering.Tests.Application.Services;

public class TagServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TagService _service;

    public TagServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new TagService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateTag_ShouldSucceed_WhenValidTag()
    {
        // Arrange
        var tag = TagTestData.CreateValidTag();
        var dto = new TagCreateDto { Name = tag.Name };
        var readDto = new TagReadDto { Id = tag.Id, Name = tag.Name };

        _mockMapper.Setup(m => m.Map<Tag>(dto)).Returns(tag);
        _mockMapper.Setup(m => m.Map<TagReadDto>(tag)).Returns(readDto);
        _mockMapper.Setup(m => m.Map<TagCreatedEvent>(tag)).Returns(new TagCreatedEvent());

        // Act
        var result = await _service.CreateTag(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(tag.Name);
        _dbContext.Tags.Should().ContainSingle(t => t.Name == tag.Name);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TagCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateTag_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var dto = new TagCreateDto { Name = "Invalid" };

        _mockMapper
            .Setup(m => m.Map<Tag>(It.IsAny<TagCreateDto>()))
            .Throws(new Exception("Database failure"));

        // Act
        var result = await _service.CreateTag(dto);

        // Assert
        result.ShouldFailWith<TagReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Database failure"
        );
    }

    [Fact]
    public async Task GetTag_ShouldSucceed_WhenTagExists()
    {
        // Arrange
        var tag = TagTestData.CreateValidTag();
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        var readDto = new TagReadDto { Id = tag.Id, Name = tag.Name };

        _mockMapper.Setup(m => m.Map<TagReadDto>(tag)).Returns(readDto);

        // Act
        var result = await _service.GetTag(tag.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(tag.Id);
        result.Data.Name.Should().Be(tag.Name);
    }

    [Fact]
    public async Task GetTag_ShouldFail_WhenTagDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.GetTag(nonExistentId);

        // Assert
        result.ShouldFailWith<TagReadDto>(
            HttpStatusCode.NotFound,
            "Tag not found."
        );
    }

    [Fact]
    public async Task UpdateTag_ShouldSucceed_WhenTagExists()
    {
        // Arrange
        var tag = TagTestData.CreateValidTag();
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        var updateDto = new TagUpdateDto { Name = TagTestData.UpdatedTagName };

        var updatedDto = new TagReadDto
        {
            Id = tag.Id,
            Name = TagTestData.UpdatedTagName
        };

        _mockMapper.Setup(m => m.Map(updateDto, tag)).Callback(() => tag.Name = updateDto.Name!);
        _mockMapper.Setup(m => m.Map<TagReadDto>(tag)).Returns(updatedDto);
        _mockMapper.Setup(m => m.Map<TagUpdatedEvent>(tag)).Returns(new TagUpdatedEvent());

        // Act
        var result = await _service.UpdateTag(updateDto, tag.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(tag.Id);
        result.Data.Name.Should().Be(TagTestData.UpdatedTagName);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TagUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTag_ShouldFail_WhenTagDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateDto = new TagUpdateDto { Name = "DoesNotMatter" };

        // Act
        var result = await _service.UpdateTag(updateDto, nonExistentId);

        // Assert
        result.ShouldFailWith<TagReadDto>(
            HttpStatusCode.NotFound,
            "Tag not found."
        );
    }

    [Fact]
    public async Task UpdateTag_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var tag = TagTestData.CreateValidTag();
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        var updateDto = new TagUpdateDto { Name = TagTestData.UpdatedTagName };

        // Force mapper to throw (simulating unexpected failure)
        _mockMapper.Setup(m => m.Map(updateDto, tag))
                   .Throws(new Exception("Database error"));

        // Act
        var result = await _service.UpdateTag(updateDto, tag.Id);

        // Assert
        result.ShouldFailWith<TagReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Database error"
        );
    }

    [Fact]
    public async Task DeleteTag_ShouldSucceed_WhenTagExists()
    {
        // Arrange
        var tag = TagTestData.CreateValidTag();
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<TagDeletedEvent>(tag)).Returns(new TagDeletedEvent());

        // Act
        var result = await _service.DeleteTag(tag.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().BeTrue();

        var softDeletedTag = await _dbContext.Tags.FindAsync(tag.Id);
        softDeletedTag!.IsDeleted.Should().BeTrue();
        softDeletedTag.IsUsed.Should().BeFalse();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TagDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTag_ShouldFail_WhenTagDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _service.DeleteTag(nonExistentId);

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.NotFound,
            "Tag not found."
        );
    }

    [Fact]
    public async Task DeleteTag_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var tag = TagTestData.CreateValidTag();
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();

        // Force event handler to throw to simulate downstream failure
        _mockEventHandler.Setup(e => e.HandleEventAsync(It.IsAny<TagDeletedEvent>()))
                         .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.DeleteTag(tag.Id);

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Database error"
        );
    }
}
