namespace RestaurantOrdering.Events.Domain.SubCategories;

public class SubCategoryDeletedEvent : BaseEvent
{
    public Guid SubCategoryId { get; set; }

    public override string GetEventType() => nameof(SubCategoryDeletedEvent);
}
