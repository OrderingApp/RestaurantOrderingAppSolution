using Application.Dtos.Areas;
using Application.Dtos.Common;
using Application.Dtos.Tables;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using System.Net;

public class AreaServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly AreaService _areaService;

    public AreaServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockMapper = new Mock<IMapper>();
        _mockEventHandler = new Mock<IEventHandlerService>();

        _areaService = new AreaService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateArea_ShouldSucceed_WhenNameIsValid()
    {
        // Arrange
        var dto = new AreaCreateDto { Name = "Garden View" };
        var area = new Area { Id = Guid.NewGuid(), Name = dto.Name };
        var readDto = new AreaReadDto { Id = area.Id, Name = dto.Name, Tables = new() };

        _mockMapper.Setup(m => m.Map<Area>(dto)).Returns(area);
        _mockMapper.Setup(m => m.Map<AreaReadDto>(It.IsAny<Area>())).Returns(readDto);

        // Act
        var result = await _areaService.CreateArea(dto, Guid.NewGuid());

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be("Garden View");

        var saved = await _dbContext.Areas.FirstOrDefaultAsync(a => a.Name == "Garden View");
        saved.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateArea_ShouldFail_WhenNameIsEmpty(string? name)
    {
        // Arrange
        var dto = new AreaCreateDto { Name = name ?? string.Empty };

        // Act
        var result = await _areaService.CreateArea(dto, Guid.NewGuid());

        // Assert
        result.ShouldFailWith(HttpStatusCode.BadRequest, "Area name is required");
    }

    [Fact]
    public async Task CreateArea_ShouldFail_WhenNameAlreadyExists()
    {
        // Arrange
        var existingArea = new Area { Id = Guid.NewGuid(), Name = "Main Hall" };
        _dbContext.Areas.Add(existingArea);
        await _dbContext.SaveChangesAsync();

        var dto = new AreaCreateDto { Name = "Main Hall" };

        // Act
        var result = await _areaService.CreateArea(dto, Guid.NewGuid());

        // Assert
        result.ShouldFailWith(HttpStatusCode.Conflict, "Area name already exists");
    }
}
