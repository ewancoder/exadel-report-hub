using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class Plans : IModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public List<Item> items { get; set; } = [];
    public int Amount { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ObjectId Id { get; set; }
}
