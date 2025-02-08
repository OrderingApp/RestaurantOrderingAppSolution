namespace RestaurantOrdering.Events.Domain.Orders.DiscountsOrder;

public class OrderDiscountAppliedEvent : BaseEvent
{
    public Guid OrderId { get; set; }

    public override string GetEventType() => nameof(OrderDiscountAppliedEvent);
}
