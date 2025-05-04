using ExportPro.Common.Shared.Enums;

namespace ExportPro.Common.Shared.Helpers;

public static class PermissionChecker
{
    public static bool HasPermission(UserRole role, Resource resource, CrudAction action)
    {
        if (!RolePermissions.Matrix.TryGetValue(role, out var permissions))
            return false;

        var resourcePermission = permissions.FirstOrDefault(p => p.Resource == resource);
        return resourcePermission?.AllowedActions?.Contains(action) ?? false;
    }
}

