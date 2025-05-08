using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.StorageService.Models.Models;

public sealed class Client : AuditModel, IModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; } = false;
    public List<Item>? Items { get; set; } = [];
    public List<Plans>? Plans { get; set; } = [];
    public Guid? OwnerId { get; set; }
    public List<Guid>? ClientAdmins { get; set; } = [];
    public ObjectId Id { get; set; }
}
