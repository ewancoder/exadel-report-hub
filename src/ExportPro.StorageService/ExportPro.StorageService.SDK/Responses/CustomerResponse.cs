using ExportPro.Common.Models.MongoDB;

namespace ExportPro.StorageService.SDK.Responses;

public sealed class CustomerResponse : AuditModel
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Guid CountryId { get; set; }
    public bool IsDeleted { get; set; }
}
