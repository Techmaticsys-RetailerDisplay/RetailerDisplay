namespace RetailerDisplay.Application.Common;

/// <summary>A page of results returned by list endpoints.</summary>
public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);

/// <summary>Common paging/search query parameters.</summary>
public record PageQuery(int Page = 1, int PageSize = 25, string? Search = null)
{
    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize is < 1 or > 200 ? 25 : PageSize;
    public int Skip => (SafePage - 1) * SafePageSize;
}
