using System.Net;
using Application.Dtos.Tables;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Tables;
using RestaurantOrdering.Tests.TestData;
using Xunit;

namespace RestaurantOrdering.Tests.Application.Services;

public class TableServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TableService _service;

    public TableServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new TableService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateTable_ShouldFail_WhenAreaDoesNotExist()
    {
        var dto = new TableCreateDto
        {
            Name = "P10",
            Capacity = 4,
            AreaId = Guid.NewGuid()
        };

        var result = await _service.CreateTable(dto);

        result.ShouldFailWith<TableReadDto>(
            HttpStatusCode.BadRequest,
            "The specified AreaId does not exist."
        );
    }

    [Fact]
    public async Task CreateTable_ShouldSucceed_WhenValidAreaExists()
    {
        var area = AreaTestData.CreateValidArea();
        await _dbContext.Areas.AddAsync(area);
        await _dbContext.SaveChangesAsync();

        var dto = new TableCreateDto
        {
            Name = "P1",
            Capacity = 6,
            AreaId = area.Id
        };

        var createdTable = TableTestData.CreateCorrectTable();
        _mockMapper.Setup(m => m.Map<Table>(dto)).Returns(createdTable);

        var createdDto = new TableReadDto
        {
            Id = createdTable.Id,
            Name = createdTable.Name,
            Capacity = createdTable.Capacity,
        };
        _mockMapper.Setup(m => m.Map<TableReadDto>(createdTable)).Returns(createdDto);
        _mockMapper.Setup(m => m.Map<TableCreatedEvent>(createdTable)).Returns(new TableCreatedEvent());

        var result = await _service.CreateTable(dto);

        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data!.Name.Should().Be("P1");
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TableCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task GetTable_ShouldFail_WhenNotFound()
    {
        var result = await _service.GetTable(Guid.NewGuid());

        result.ShouldFailWith<TableSummaryDto>(
            HttpStatusCode.NotFound,
            "Table not found or deleted."
        );
    }

    [Fact]
    public async Task GetTable_ShouldSucceed_WhenTableExists()
    {
        var area = AreaTestData.CreateValidArea();
        await _dbContext.Areas.AddAsync(area);
        await _dbContext.SaveChangesAsync();

        var table = TableTestData.CreateCorrectTable();
        table.AreaId = area.Id;

        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        var dto = new TableSummaryDto { Id = table.Id, Name = table.Name };
        _mockMapper.Setup(m => m.Map<TableSummaryDto>(table)).Returns(dto);

        var result = await _service.GetTable(table.Id);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(table.Id);
    }

    [Fact]
    public async Task GetTables_ShouldReturnAll()
    {
        var table1 = TableTestData.CreateCorrectTable();
        var table2 = TableTestData.CreateOngoingTable();

        await _dbContext.Tables.AddRangeAsync(table1, table2);
        await _dbContext.SaveChangesAsync();

        var dtos = new List<TableReadDto>
        {
            new() { Id = table1.Id, Name = table1.Name, Capacity = table1.Capacity },
            new() { Id = table2.Id, Name = table2.Name, Capacity = table2.Capacity }
        };
        _mockMapper.Setup(m => m.Map<List<TableReadDto>>(It.IsAny<List<Table>>())).Returns(dtos);

        var result = await _service.GetTables();

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateTable_ShouldFail_WhenTableNotFound()
    {
        var dto = new TableUpdateDto { Name = "Updated" };

        var result = await _service.UpdateTable(Guid.NewGuid(), dto);

        result.ShouldFailWith<TableReadDto>(
            HttpStatusCode.NotFound,
            "Table not found."
        );
    }

    [Fact]
    public async Task UpdateTable_ShouldSucceed_WhenValid()
    {
        var table = TableTestData.CreateCorrectTable();
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        var dto = new TableUpdateDto { Name = "Updated" };
        var updatedDto = new TableReadDto { Id = table.Id, Name = "Updated" };

        _mockMapper.Setup(m => m.Map(dto, table)).Verifiable();
        _mockMapper.Setup(m => m.Map<TableReadDto>(table)).Returns(updatedDto);
        _mockMapper.Setup(m => m.Map<TableUpdatedEvent>(table)).Returns(new TableUpdatedEvent());

        var result = await _service.UpdateTable(table.Id, dto);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be("Updated");
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TableUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatus_ShouldFail_WhenTableNotFound()
    {
        var result = await _service.UpdateStatus(Guid.NewGuid(), TableStatus.Ongoing);

        result.ShouldFailWith<TableReadDto>(
            HttpStatusCode.NotFound,
            "Table not found or has been deleted."
        );
    }

    [Fact]
    public async Task UpdateStatus_ShouldFail_WhenAlreadySameStatus()
    {
        var table = TableTestData.CreateOngoingTable();
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        var result = await _service.UpdateStatus(table.Id, TableStatus.Ongoing);

        result.ShouldFailWith<TableReadDto>(
            HttpStatusCode.BadRequest,
            "Table already has the requested status."
        );
    }

    [Fact]
    public async Task TogglePreparation_ShouldFail_WhenTableNotFound()
    {
        var result = await _service.TogglePreparation(Guid.NewGuid());

        result.ShouldFailWith<TableReadDto>(
            HttpStatusCode.NotFound,
            "Table not found or has been deleted."
        );
    }

    [Fact]
    public async Task TogglePreparation_ShouldSucceed_WhenValid()
    {
        var table = TableTestData.CreateCorrectTable();
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        var dto = new TableReadDto { Id = table.Id, Name = table.Name, IsPrepared = !table.IsPrepared };
        _mockMapper.Setup(m => m.Map<TableReadDto>(table)).Returns(dto);

        var result = await _service.TogglePreparation(table.Id);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.IsPrepared.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTable_ShouldFail_WhenNotFound()
    {
        var result = await _service.DeleteTable(Guid.NewGuid());

        result.ShouldFailWith<bool>(
            HttpStatusCode.NotFound,
            "Table not found."
        );
    }

    [Fact]
    public async Task DeleteTable_ShouldSucceed_WhenValid()
    {
        var table = TableTestData.CreateCorrectTable();
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<TableDeletedEvent>(table)).Returns(new TableDeletedEvent());

        var result = await _service.DeleteTable(table.Id);

        result.ShouldBeSuccessful(HttpStatusCode.OK);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TableDeletedEvent>()), Times.Once);

        var deleted = await _dbContext.Tables.FindAsync(table.Id);
        deleted!.IsDeleted.Should().BeTrue();
    }
}
