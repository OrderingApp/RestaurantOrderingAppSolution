using Application.Dtos.CustomerInformations;
using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class CustomerInformationTestData
{
    public const string DefaultPhone = "123456789";
    public const string UpdatedPhone = "987654321";

    public static CustomerInformation CreateValidCustomer(Guid? id = null)
    {
        return new CustomerInformation
        {
            Id = id ?? Guid.NewGuid(),
            PhoneNumber = DefaultPhone,
            OrderCompletionType = OrderCompletionType.Immediate,
            OrderId = Guid.NewGuid(),
            Order = new Order()
        };
    }

    public static CustomerInformationUpdateDto CreateUpdateDto() => new()
    {
        PhoneNumber = UpdatedPhone,
        OrderCompletionType = OrderCompletionType.Scheduled,
    };

    public static CustomerInformationReadDto CreateReadDto(Guid id) => new()
    {
        PhoneNumber = UpdatedPhone
    };
}
