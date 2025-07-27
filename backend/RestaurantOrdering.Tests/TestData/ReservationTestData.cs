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
        DateTime? dateTime = null,
        int capacityNeeded = 4
    ) =>
        new ReservationCreateDto
        {
            PhoneNumber = phoneNumber ?? DefaultPhoneNumber,
            Name = name ?? DefaultName,
            DateTime = dateTime ?? DateTime.UtcNow.AddHours(3),
            CapacityNeeded = capacityNeeded
        };

    public static Reservation CreateReservation(
        Guid? id = null,
        string? phoneNumber = null,
        string? name = null,
        DateTime? dateTime = null,
        bool? isAssigned = null,
        int capacityNeeded = 4
    ) =>
        new Reservation
        {
            Id = id ?? Guid.NewGuid(),
            PhoneNumber = phoneNumber ?? DefaultPhoneNumber,
            Name = name ?? DefaultName,
            DateTime = dateTime ?? DateTime.UtcNow.AddHours(3),
            CapacityNeeded = capacityNeeded,
            IsAssigned = isAssigned ?? false
        };

    public static ReservationReadDto CreateReservationReadDto(Reservation reservation) =>
        new ReservationReadDto
        {
            Id = reservation.Id,
            PhoneNumber = reservation.PhoneNumber,
            Name = reservation.Name,
            DateTime = reservation.DateTime,
            CapacityNeeded = reservation.CapacityNeeded,
            IsAssigned = reservation.IsAssigned
        };
}
