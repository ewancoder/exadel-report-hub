using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class Plans : AuditModel, IModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public List<Item> items { get; set; } = [];
    public int Amount { get; set; }
    public ObjectId Id { get; set; }
}
