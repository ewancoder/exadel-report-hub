using System.Text.Json.Serialization;

namespace ExportPro.StorageService.SDK.PaginationParams;

public class PaginatedListDto<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    // convenience ctor for server-side code
    public PaginatedListDto(
        List<T> items,
        int count,          // ← name no longer matters to Newtonsoft
        int pageNumber,
        int totalPages)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        TotalPages = totalPages;
    }
}
