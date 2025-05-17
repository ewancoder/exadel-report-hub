using System.Text.Json.Serialization;

namespace ExportPro.StorageService.SDK.PaginationParams;

public sealed class PaginatedListDto<T>
{
    public PaginatedListDto(List<T> items, int count, int pageNumber, int totalPages)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        TotalPages = totalPages;
    }

    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
