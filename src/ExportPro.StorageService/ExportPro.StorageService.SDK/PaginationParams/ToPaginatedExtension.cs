namespace ExportPro.StorageService.SDK.PaginationParams;

public static class ToPaginatedExtension
{
    public static PaginatedList<T> ToPaginatedList<T>(this List<T> list, int pageNumber, int pageSize)
    {
        var count = list.Count;
        list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PaginatedList<T>(list, count, pageNumber, pageSize);
    }
}
