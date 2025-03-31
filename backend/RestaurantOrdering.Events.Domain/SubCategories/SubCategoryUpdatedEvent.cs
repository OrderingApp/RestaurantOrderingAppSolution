namespace RestaurantOrdering.Events.Domain.SubCategories;

public class SubCategoryUpdatedEvent : BaseEvent
{
    public Guid SubCategoryId { get; set; }

    public override string GetEventType() => nameof(SubCategoryDeletedEvent);
}
