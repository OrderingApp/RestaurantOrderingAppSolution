namespace Application.Dtos.Common;

public class PagedResultDto<T> : ResultDto<List<T>>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public PagedResultDto(List<T> data, int totalCount, int page, int pageSize) 
        : base(data)
    {
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
