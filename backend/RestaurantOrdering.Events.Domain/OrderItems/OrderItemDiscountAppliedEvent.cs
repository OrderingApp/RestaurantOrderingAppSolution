namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemDiscountAppliedEvent : BaseEvent
{
    public Guid OrderItemId { get; set; }
    public decimal DiscountPercentage { get; set; }

    public override string GetEventType() => nameof(OrderItemDiscountAppliedEvent);
}
