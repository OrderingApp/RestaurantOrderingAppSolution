using Application.Dtos.CustomerInformations;
using Application.Dtos.OrderItemIngredients;
using Application.Dtos.OrderItems;
using Application.Dtos.Orders.OrderDelivery;
using Application.Dtos.Orders.OrderTakeAway;
using Domain;

public static class OrderTestData
{
    public static Order CreateOrder(
            Guid? id = null,
            OrderType type = OrderType.DineIn,
            OrderStatus status = OrderStatus.Ongoing,
            Guid? tableId = null,
            decimal totalAmount = 100,
            decimal discount = 0
        )
    {
        return new Order
        {
            Id = id ?? Guid.NewGuid(),
            Type = type,
            Status = status,
            TableId = tableId,
            TotalAmount = totalAmount,
            Discount = discount,
            OrderItems = new List<OrderItem>(),
            Payments = new List<Payment>(),
            CustomerInformation = null
        };
    }

    public static TakeawayOrderCreateDto CreateTakeawayOrderCreateDto(
    Guid menuItemId,
    Guid ingredientId,
    string phoneNumber = "123456789",
    string? specialInstructions = "Extra spicy"
)
    {
        return new TakeawayOrderCreateDto
        {
            CustomerInformation = new CustomerInformationCreateDto
            {
                PhoneNumber = phoneNumber,
                AdditionalInstructions = "Please ring bell",
                Address = null,
                OrderCompletionType = OrderCompletionType.Immediate,
                ExpectedOrderCompletion = null
            },
            OrderItems =
            [
                new OrderItemCreateDto
            {
                MenuItemId = menuItemId,
                ExtraIngredients =
                [
                    new OrderItemIngredientAddDto
                    {
                        IngredientId = ingredientId,
                        Quantity = 1
                    }
                ],
                RemovedIngredientIds = [],
                SpecialInstructions = specialInstructions
            }
            ]
        };
    }

    public static DeliveryOrderCreateDto CreateDeliveryOrderCreateDto(Guid menuItemId, Guid ingredientId)
    {
        return new DeliveryOrderCreateDto
        {
            CustomerInformation = new CustomerInformationCreateDto
            {
                PhoneNumber = "123456789",
                AdditionalInstructions = "Leave at the door",
                Address = "123 Street",
                OrderCompletionType = OrderCompletionType.Immediate,
                ExpectedOrderCompletion = null
            },
            OrderItems = new List<OrderItemCreateDto>
        {
            new OrderItemCreateDto
            {
                MenuItemId = menuItemId,
                SpecialInstructions = "Extra sauce",
                ExtraIngredients = new List<OrderItemIngredientAddDto>
                {
                    new() { IngredientId = ingredientId, Quantity = 1 }
                },
                RemovedIngredientIds = new List<Guid>()
            }
        }
        };
    }


    public static Order CreateOrderWithItems(Guid? orderId = null, List<OrderItem>? items = null)
    {
        var order = CreateOrder(id: orderId);
        if (items != null)
        {
            order.OrderItems.AddRange(items);
            order.TotalAmount = items.Sum(i => i.Price);
        }
        return order;
    }

    public static Order CreateOrderWithPayments(Guid? orderId = null, List<Payment>? payments = null)
    {
        var order = CreateOrder(id: orderId);
        if (payments != null)
        {
            order.Payments.AddRange(payments);
        }
        return order;
    }
}
