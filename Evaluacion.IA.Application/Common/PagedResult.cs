namespace Evaluacion.IA.Application.Common;

public sealed class PagedResult<T>
{
    public List<T> Items { get; private set; }
    public int TotalCount { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult(List<T> items, int totalCount, int page, int pageSize)
    {
        Items = items ?? new List<T>();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
    }

    public PagedResult()
    {
        Items = new List<T>();
        TotalCount = 0;
        Page = 1;
        PageSize = 10;
        TotalPages = 0;
    }
}
