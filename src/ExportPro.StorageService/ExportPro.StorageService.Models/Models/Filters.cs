using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.Models.Models;

public class Filters
{
    public int Top { get; set; } = 10;
    public int Skip { get; set; } = 0;
    public OrderBy OrderBy { get; set; } = OrderBy.Ascending;
}
