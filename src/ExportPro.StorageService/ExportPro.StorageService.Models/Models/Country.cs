using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class Country : AuditModel, IModel
{
    public required string Name { get; set; }
    public string? Code { get; set; }
    public ObjectId CurrencyId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public ObjectId Id { get; set; }
}
