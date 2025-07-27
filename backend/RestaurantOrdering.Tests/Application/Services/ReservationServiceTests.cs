using Application.Dtos.Reservations;
using Application.Services;
using AutoMapper;
using Domain;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantOrdering.Events.Application.Contracts;
using RestaurantOrdering.Events.Domain.Reservations;
using RestaurantOrdering.Tests.TestData;
using System.Net;

namespace RestaurantOrdering.Tests.Application.Services;

public class ReservationServiceTests
{
    private readonly RestaurantOrderingContext _dbContext;
    private readonly Mock<IEventHandlerService> _mockEventHandler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantOrderingContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new RestaurantOrderingContext(options);
        _mockEventHandler = new Mock<IEventHandlerService>();
        _mockMapper = new Mock<IMapper>();

        _service = new ReservationService(_dbContext, _mockEventHandler.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateReservation_ShouldSucceed_WhenValidDto()
    {
        // Arrange
        var dto = ReservationTestData.CreateReservationCreateDto();
        var reservation = ReservationTestData.CreateReservation(phoneNumber: dto.PhoneNumber, name: dto.Name, dateTime: dto.DateTime, capacityNeeded: dto.CapacityNeeded);
        var reservationReadDto = ReservationTestData.CreateReservationReadDto(reservation);

        _mockMapper.Setup(m => m.Map<Reservation>(dto)).Returns(reservation);
        _mockMapper.Setup(m => m.Map<ReservationReadDto>(reservation)).Returns(reservationReadDto);
        _mockMapper.Setup(m => m.Map<ReservationCreatedEvent>(reservation)).Returns(new ReservationCreatedEvent());

        // Act
        var result = await _service.CreateReservation(dto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.Created);
        result.Data.Should().NotBeNull();
        result.Data!.PhoneNumber.Should().Be(dto.PhoneNumber);
        result.Data.Name.Should().Be(dto.Name);

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<ReservationCreatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task CreateReservation_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var dto = ReservationTestData.CreateReservationCreateDto();

        // Force mapper to throw an exception when mapping to Reservation
        _mockMapper.Setup(m => m.Map<Reservation>(dto)).Throws(new Exception("Database error"));

        // Act
        var result = await _service.CreateReservation(dto);

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occured: Database error"
        );

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<ReservationCreatedEvent>()), Times.Never);
    }

    [Fact]
    public async Task GetReservation_ShouldSucceed_WhenReservationExists()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();
        var reservationDto = ReservationTestData.CreateReservationReadDto(reservation);

        _mockMapper.Setup(m => m.Map<ReservationReadDto>(reservation))
            .Returns(reservationDto);

        // Act
        var result = await _service.GetReservation(reservation.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Id.Should().Be(reservation.Id);
        result.Data.Name.Should().Be(reservation.Name);
    }

    [Fact]
    public async Task GetReservation_ShouldFail_WhenReservationNotFound()
    {
        // Act
        var result = await _service.GetReservation(Guid.NewGuid());

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.NotFound,
            "Reservation not found."
        );
    }

    [Fact]
    public async Task GetReservation_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<ReservationReadDto>(It.IsAny<Reservation>()))
            .Throws(new Exception("Mapping failed"));

        // Act
        var result = await _service.GetReservation(reservation.Id);

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occured: Mapping failed"
        );
    }

    [Fact]
    public async Task GetReservationsByDate_ShouldSucceed_WhenReservationsExist()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;

        var reservations = new List<Reservation>
    {
        ReservationTestData.CreateReservation(dateTime: date.AddHours(10)),
        ReservationTestData.CreateReservation(dateTime: date.AddHours(12))
    };

        await _dbContext.Reservations.AddRangeAsync(reservations);
        await _dbContext.SaveChangesAsync();

        var reservationDtos = reservations.Select(r => new ReservationReadDto
        {
            Id = r.Id,
            PhoneNumber = r.PhoneNumber,
            Name = r.Name,
            DateTime = r.DateTime,
            CapacityNeeded = r.CapacityNeeded,
            IsAssigned = r.IsAssigned
        }).ToList();

        _mockMapper.Setup(m => m.Map<List<ReservationReadDto>>(reservations))
            .Returns(reservationDtos);

        // Act
        var result = await _service.GetReservationsByDate(date);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Should().HaveCount(2);
        result.Data.Select(r => r.Id).Should().BeEquivalentTo(reservations.Select(r => r.Id));
    }

    [Fact]
    public async Task GetReservationsByDate_ShouldSucceed_WithEmptyList_WhenNoReservations()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;

        _mockMapper.Setup(m => m.Map<List<ReservationReadDto>>(It.IsAny<List<Reservation>>()))
            .Returns(new List<ReservationReadDto>());

        // Act
        var result = await _service.GetReservationsByDate(date);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReservationsByDate_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var date = DateTime.UtcNow.Date;

        _mockMapper.Setup(m => m.Map<List<ReservationReadDto>>(It.IsAny<List<Reservation>>()))
            .Throws(new Exception("Mapping failed"));

        // Act
        var result = await _service.GetReservationsByDate(date);

        // Assert
        result.ShouldFailWith<List<ReservationReadDto>>(
            HttpStatusCode.InternalServerError,
            "An error occurred: Mapping failed"
        );
    }

    [Fact]
    public async Task AssignTableToReservation_ShouldSucceed_WhenReservationAndTableExist()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        var table = TableTestData.CreateCorrectTable();

        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        var reservationDto = new ReservationReadDto
        {
            Id = reservation.Id,
            PhoneNumber = reservation.PhoneNumber,
            Name = reservation.Name,
            DateTime = reservation.DateTime,
            CapacityNeeded = reservation.CapacityNeeded,
            IsAssigned = true,
            TableName = table.Name
        };

        _mockMapper.Setup(m => m.Map<TableAssignedToReservationEvent>(
            It.Is<(Reservation, Guid)>(tuple => tuple.Item1.Id == reservation.Id && tuple.Item2 == table.Id)))
            .Returns(new TableAssignedToReservationEvent());

        _mockMapper.Setup(m => m.Map<ReservationReadDto>(reservation))
            .Returns(reservationDto);

        // Act
        var result = await _service.AssignTableToReservation(reservation.Id, table.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.TableName.Should().Be(table.Name);
        result.Data.IsAssigned.Should().BeTrue();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TableAssignedToReservationEvent>()), Times.Once);
    }

    [Fact]
    public async Task AssignTableToReservation_ShouldFail_WhenReservationNotFound()
    {
        // Arrange
        var table = TableTestData.CreateCorrectTable();
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.AssignTableToReservation(Guid.NewGuid(), table.Id);

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.NotFound,
            "Reservation not found."
        );
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TableAssignedToReservationEvent>()), Times.Never);
    }

    [Fact]
    public async Task AssignTableToReservation_ShouldFail_WhenTableNotFound()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation(isAssigned: false);
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.AssignTableToReservation(reservation.Id, Guid.NewGuid());

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.NotFound,
            "Table not found."
        );
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<TableAssignedToReservationEvent>()), Times.Never);
    }

    [Fact]
    public async Task AssignTableToReservation_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation(isAssigned: false);
        var table = TableTestData.CreateCorrectTable();

        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.Tables.AddAsync(table);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<TableAssignedToReservationEvent>(
            It.IsAny<(Reservation, Guid)>())).Throws(new Exception("Mapping failed"));

        // Act
        var result = await _service.AssignTableToReservation(reservation.Id, table.Id);

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occured: Mapping failed"
        );
    }

    [Fact]
    public async Task UpdateReservation_ShouldSucceed_WhenReservationExists()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();

        var updateDto = new ReservationUpdateDto
        {
            PhoneNumber = "987654321",
            Name = "Updated Name",
            CapacityNeeded = 5,
            DateTime = reservation.DateTime.AddHours(2)
        };

        var updatedReservationDto = new ReservationReadDto
        {
            Id = reservation.Id,
            PhoneNumber = updateDto.PhoneNumber!,
            Name = updateDto.Name!,
            CapacityNeeded = updateDto.CapacityNeeded!.Value,
            DateTime = updateDto.DateTime!.Value,
            IsAssigned = reservation.IsAssigned,
        };

        _mockMapper.Setup(m => m.Map(updateDto, reservation));
        _mockMapper.Setup(m => m.Map<ReservationReadDto>(reservation))
            .Returns(updatedReservationDto);
        _mockMapper.Setup(m => m.Map<ReservationUpdatedEvent>(reservation))
            .Returns(new ReservationUpdatedEvent());

        // Act
        var result = await _service.UpdateReservation(reservation.Id, updateDto);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.OK);
        result.Data!.Name.Should().Be(updateDto.Name);
        result.Data.CapacityNeeded.Should().Be(updateDto.CapacityNeeded);
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<ReservationUpdatedEvent>()), Times.Once);
    }

    [Fact]
    public async Task UpdateReservation_ShouldFail_WhenReservationNotFound()
    {
        // Arrange
        var updateDto = new ReservationUpdateDto
        {
            PhoneNumber = "987654321",
            Name = "Updated Name"
        };

        // Act
        var result = await _service.UpdateReservation(Guid.NewGuid(), updateDto);

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.NotFound,
            "Reservation not found."
        );
        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<ReservationUpdatedEvent>()), Times.Never);
    }

    [Fact]
    public async Task UpdateReservation_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();

        var updateDto = new ReservationUpdateDto { Name = "Updated" };

        _mockMapper.Setup(m => m.Map(updateDto, reservation))
            .Throws(new Exception("Mapping failed"));

        // Act
        var result = await _service.UpdateReservation(reservation.Id, updateDto);

        // Assert
        result.ShouldFailWith<ReservationReadDto>(
            HttpStatusCode.InternalServerError,
            "An error occured: Mapping failed"
        );
    }

    [Fact]
    public async Task DeleteReservation_ShouldSucceed_WhenReservationExists()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<ReservationDeletedEvent>(reservation))
            .Returns(new ReservationDeletedEvent());

        // Act
        var result = await _service.DeleteReservation(reservation.Id);

        // Assert
        result.ShouldBeSuccessful(HttpStatusCode.NoContent);
        result.Data.Should().BeTrue();

        var deleted = await _dbContext.Reservations.FindAsync(reservation.Id);
        deleted.Should().BeNull();

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<ReservationDeletedEvent>()), Times.Once);
    }

    [Fact]
    public async Task DeleteReservation_ShouldFail_WhenReservationNotFound()
    {
        // Act
        var result = await _service.DeleteReservation(Guid.NewGuid());

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.NotFound,
            "Reservation not found."
        );

        _mockEventHandler.Verify(e => e.HandleEventAsync(It.IsAny<ReservationDeletedEvent>()), Times.Never);
    }

    [Fact]
    public async Task DeleteReservation_ShouldFail_WhenExceptionThrown()
    {
        // Arrange
        var reservation = ReservationTestData.CreateReservation();
        await _dbContext.Reservations.AddAsync(reservation);
        await _dbContext.SaveChangesAsync();

        _mockMapper.Setup(m => m.Map<ReservationDeletedEvent>(It.IsAny<Reservation>()))
            .Throws(new Exception("Event mapping failed"));

        // Act
        var result = await _service.DeleteReservation(reservation.Id);

        // Assert
        result.ShouldFailWith<bool>(
            HttpStatusCode.InternalServerError,
            "An error occured: Event mapping failed"
        );

        // The reservation is still deleted because SaveChanges succeeded before mapping.
        var deleted = await _dbContext.Reservations.FindAsync(reservation.Id);
        deleted.Should().BeNull();
    }

}
