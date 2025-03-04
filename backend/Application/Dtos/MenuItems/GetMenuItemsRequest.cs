namespace Application.Dtos.MenuItems;

public class GetMenuItemsRequest
{
    public Guid? MenuCategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public List<Guid>? TagIds { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}