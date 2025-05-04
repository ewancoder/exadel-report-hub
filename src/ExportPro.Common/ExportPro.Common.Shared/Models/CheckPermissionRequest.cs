using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.Common.Shared.Models;
public class CheckPermissionRequest
{
    public string? UserId { get; set; }
    public string? ClientId { get; set; }
    public Resource Resource { get; set; }
    public CrudAction Action { get; set; }
}

public class CheckPermissionResponse
{
    public bool HasPermission { get; set; }
}