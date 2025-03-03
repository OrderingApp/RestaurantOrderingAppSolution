namespace Application.Dtos.MenuItems;

public class GetMenuItemsRequest
{
    public Guid? MenuCategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public List<Guid>? TagIds { get; set; }
}