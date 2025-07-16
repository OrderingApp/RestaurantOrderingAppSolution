using Domain;

namespace RestaurantOrdering.Tests.TestData;

public static class PaymentTestData
{
    public static Payment CreatePayment(
        Guid? id = null,
        Guid? orderId = null,
        decimal amount = 100m,
        PaymentMethod paymentMethod = PaymentMethod.Cash
    )
    {
        return new Payment
        {
            Id = id ?? Guid.NewGuid(),
            OrderId = orderId ?? Guid.NewGuid(),
            Amount = amount,
            PaymentMethod = paymentMethod,
            PaidAt = DateTime.UtcNow
        };
    }
}
