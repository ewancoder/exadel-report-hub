using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;


namespace ExportPro.Auth.SDK.DTOs;
public class CheckPermissionRequest
{
    public ObjectId UserId { get; set; }
    public ObjectId ClientId { get; set; }
    public Resource Resource { get; set; }
    public CrudAction Action { get; set; }
}