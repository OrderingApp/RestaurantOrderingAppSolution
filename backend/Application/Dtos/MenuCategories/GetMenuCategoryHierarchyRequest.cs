using Microsoft.AspNetCore.Mvc;

namespace Application.Dtos.MenuCategories;

public class GetMenuCategoryHierarchyRequest
{
    public Guid? MenuCategoryId { get; set; }
    public Guid? SubCategoryId { get; set; }
    public List<Guid>? TagIds { get; set; }

    [FromQuery(Name = "page")]
    public int? Page { get; set; }

    [FromQuery(Name = "page-size")]
    public int? PageSize { get; set; }
}
