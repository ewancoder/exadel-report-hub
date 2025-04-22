using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Models;

namespace ExportPro.Common.Shared.Helpers;

public static class PermissionChecker
{
    public static bool HasPermission(Guid roleId, Resource resource, CrudAction action)
    {
        if (!RoleMappings.GuidToRole.TryGetValue(roleId, out var role))
            return false;

        if (!RolePermissions.Matrix.TryGetValue(role, out var permissions))
            return false;

        var resourcePermission = permissions.FirstOrDefault(p => p.Resource == resource);
        return resourcePermission?.AllowedActions?.Contains(action) ?? false;
    }
}
