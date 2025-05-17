using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.SDK.ModelFilters;

public class InvoiceFilter : PaginationParameters
{
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public CustomerFilter? Customer { get; set; }
}
public class CustomerFilter
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public Guid? CountryId { get; set; }
}