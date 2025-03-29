namespace RestaurantOrdering.Events.Domain.Orders.CreatingOrder;

public class DeliveryOrderCreatedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    //public DateTime OrderDateTime { get; set; }
    //public string PhoneNumber { get; set; } = null!;
    //public string Address { get; set; } = null!;
    //public string? AdditionalInstructions { get; set; }
    //public decimal TotalAmount { get; set; }

    //public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public override string GetEventType() => nameof(DeliveryOrderCreatedEvent);
}
