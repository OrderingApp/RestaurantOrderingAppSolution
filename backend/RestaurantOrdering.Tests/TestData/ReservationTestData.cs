using Application.Dtos.Reservations;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class ReservationTestData
{
    public const string DefaultPhoneNumber = "123456789";
    public const string DefaultName = "John Doe";

    public static ReservationCreateDto CreateReservationCreateDto(
        string? phoneNumber = null,
        string? name = null,
        DateTime? scheduledFor = null,
        int capacityNeeded = 4
    ) =>
        new ReservationCreateDto
        {
            PhoneNumber = phoneNumber ?? DefaultPhoneNumber,
            Name = name ?? DefaultName,
            ScheduledFor = scheduledFor ?? DateTime.UtcNow.AddHours(3),
            CapacityNeeded = capacityNeeded
        };

    public static Reservation CreateReservation(
        Guid? id = null,
        string? phoneNumber = null,
        string? name = null,
        DateTime? scheduledFor = null,
        Guid? tableId = null,
        int capacityNeeded = 4
    ) =>
        new Reservation
        {
            Id = id ?? Guid.NewGuid(),
            PhoneNumber = phoneNumber ?? DefaultPhoneNumber,
            Name = name ?? DefaultName,
            ScheduledFor = scheduledFor ?? DateTime.UtcNow.AddHours(3),
            CapacityNeeded = capacityNeeded,
            TableId = tableId
        };

    public static ReservationReadDto CreateReservationReadDto(Reservation reservation) =>
        new ReservationReadDto
        {
            Id = reservation.Id,
            PhoneNumber = reservation.PhoneNumber,
            Name = reservation.Name,
            ScheduledFor = reservation.ScheduledFor,
            CapacityNeeded = reservation.CapacityNeeded,
            // TableName / TableId can be set by tests when needed
        };
}
