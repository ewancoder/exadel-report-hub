
using ExportPro.Auth.SDK.Models;
using ExportPro.Common.Shared.Enums;
using Role = ExportPro.Common.Shared.Enums.Role;

namespace ExportPro.Auth.SDK.Helpers
{
    public static class Helper
    {
        public static Role ToAuthRole(this UserRole role)
        {
            return role switch
            {
                UserRole.ClientAdmin => Role.ClientAdmin,
                UserRole.Operator => Role.Operator,
                UserRole.Owner => Role.Owner,
                _ => Role.Operator
            };
        }
    }
}
