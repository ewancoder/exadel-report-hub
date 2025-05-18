using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.Auth.SDK.DTOs;

public class PermissionDTO
{
    public ObjectId ClientId { get; set; }
    public ObjectId UserId { get; set; }
    public UserRole Role { get; set; }
    public CrudAction Action { get; set; }
    public Resource? Resource { get; set; }
}
