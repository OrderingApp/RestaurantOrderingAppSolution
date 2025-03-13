using Domain;

public static class OrderTestHelper
{
    public static Order CreateOrder(Guid? id = null, Guid? tableId = null, List<OrderItem>? items = null)
    {
        return new Order
        {
            Id = id ?? Guid.NewGuid(),
            TableId = tableId,
            OrderItems = items ?? new List<OrderItem>(),
            Status = OrderStatus.Ongoing,
            Type = OrderType.DineIn
        };
    }
}
