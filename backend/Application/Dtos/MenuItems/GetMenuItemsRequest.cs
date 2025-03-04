using Microsoft.AspNetCore.Mvc;

namespace Application.Dtos.MenuItems;

public class GetMenuItemsRequest
{
    public Guid? MenuCategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public List<Guid>? TagIds { get; set; }

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "page-size")]
    public int PageSize { get; set; } = 10;
}