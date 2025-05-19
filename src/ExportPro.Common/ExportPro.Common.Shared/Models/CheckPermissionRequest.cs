using ExportPro.Common.Shared.Enums;

namespace ExportPro.Common.Shared.Models;

public class CheckPermissionRequest
{
    public Guid? UserId { get; set; }
    public Guid? ClientId { get; set; }
    public Resource Resource { get; set; }
    public CrudAction Action { get; set; }
}

public class CheckPermissionResponse
{
    public bool HasPermission { get; set; }
}
