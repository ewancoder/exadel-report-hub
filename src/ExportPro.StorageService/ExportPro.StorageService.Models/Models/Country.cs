using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public class Country : IModel
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? CurrencyId { get; set; } 
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}