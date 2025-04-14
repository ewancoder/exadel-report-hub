using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.SDK.Responses;

public class FullClientResponse
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public List<InvoiceResponse> invoices { get; set; }
    public List<Item> items { get; set; }
    public List<CustomerResponse> customers { get; set; }
}