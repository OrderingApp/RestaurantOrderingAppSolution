namespace RestaurantOrdering.Events.Domain.OrderItems;

public class OrderItemUpdatedEvent : BaseEvent
{
    public Guid OrderItemId { get; set; }
    public decimal? Discount { get; set; }
    public string? SpecialInstructions { get; set; }
    public List<Guid> ExtraIngredientIds { get; set; } = new();
    public List<Guid> RemovedIngredientIds { get; set; } = new();

    public override string GetEventType() => nameof(OrderItemUpdatedEvent);
}
