using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Models;


namespace ExportPro.Common.Shared.Helpers;
public static class RoleMappings
{
    public static readonly Dictionary<Guid, Role> GuidToRole = new()
    {
        [SystemRoles.SuperAdmin] = Role.SuperAdmin,
        [SystemRoles.Owner] = Role.Owner,
        [SystemRoles.ClientAdmin] = Role.ClientAdmin,
        [SystemRoles.Operator] = Role.Operator
    };

    public static readonly Dictionary<Role, Guid> RoleToGuid = GuidToRole
        .ToDictionary(kv => kv.Value, kv => kv.Key);
}
