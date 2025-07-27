using System.Net;
using Application.Dtos.Areas;
using Application.Dtos.Tables;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Tests.TestData;

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
    public async Task CreateArea_ShouldSucceed_WhenNameIsUnique()
    {
        // Arrange
        var dto = AreaTestData.AreaDtoTestData.CreateAreaDto();
        var newArea = AreaTestData.CreateValidArea(name: dto.Name);
        var readDto = new AreaReadDto { Id = newArea.Id, Name = dto.Name, Tables = [] };

        _mockMapper.Setup(m => m.Map<Area>(dto)).Returns(newArea);
        _mockMapper.Setup(m => m.Map<AreaReadDto>(It.IsAny<Area>())).Returns(readDto);

        // Act
        var result = await _areaService.CreateArea(dto, Guid.NewGuid());

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be(dto.Name);

        var saved = await _dbContext.Areas.FirstOrDefaultAsync(a => a.Name == dto.Name);
        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateArea_ShouldFail_WhenNameAlreadyExists()
    {
        // Arrange
        var existingArea = AreaTestData.CreateValidArea(name: AreaTestData.ExistingAreaName);
        _dbContext.Areas.Add(existingArea);
        await _dbContext.SaveChangesAsync();

        var dto = AreaTestData.AreaDtoTestData.CreateAreaDto(name: AreaTestData.ExistingAreaName);

        // Act
        var result = await _areaService.CreateArea(dto, Guid.NewGuid());

        // Assert
        result.ShouldFailWith(HttpStatusCode.Conflict, "Area name already exists");
    }

    [Fact]
    public async Task UpdateArea_ShouldFail_WhenAreaNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = AreaTestData.AreaDtoTestData.UpdateAreaDto();

        // Act
        var result = await _areaService.UpdateArea(id, dto);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Area not found");
    }

    [Fact]
    public async Task DeleteArea_ShouldFail_WhenAreaNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _areaService.DeleteArea(id);

        // Assert
        result.ShouldFailWith(HttpStatusCode.NotFound, "Area not found");
    }

    [Fact]
    public async Task DeleteArea_ShouldSucceed_WhenAreaExists()
    {
        // Arrange
        var area = AreaTestData.CreateValidArea(name: AreaTestData.TemporaryAreaName);
        _dbContext.Areas.Add(area);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _areaService.DeleteArea(area.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);

        var deleted = await _dbContext.Areas.FindAsync(area.Id);
        deleted.Should().BeNull();
    }
}
