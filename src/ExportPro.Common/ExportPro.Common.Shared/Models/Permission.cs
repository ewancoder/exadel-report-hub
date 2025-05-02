using ExportPro.Common.Shared.Enums;

namespace ExportPro.Common.Shared.Models;

public class Permission
{
    public Resource? Resource { get; set; } 
    public List<CrudAction>? AllowedActions { get; set; }
}
