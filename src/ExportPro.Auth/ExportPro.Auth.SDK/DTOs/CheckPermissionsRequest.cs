using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;


namespace ExportPro.Auth.SDK.DTOs;
public class CheckPermissionRequest
{
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }
    public Resource Resource { get; set; }
    public CrudAction Action { get; set; }
}