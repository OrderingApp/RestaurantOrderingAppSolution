namespace RestaurantOrdering.Events.Domain.SubCategories;
public class SubCategoryCreatedEvent : BaseEvent
{
    public Guid SubCategoryId { get; set; }

    public override string GetEventType() => nameof(SubCategoryCreatedEvent);
}