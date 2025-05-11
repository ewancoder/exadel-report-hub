
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Models;

namespace ExportPro.Common.Shared.Helpers;

public static class RolePermissions
{
    public static readonly Dictionary<UserRole, List<Permission>> Matrix = new()
    {
        [UserRole.Owner] =
        [
            new Permission { Resource = Resource.Clients, AllowedActions = [CrudAction.Read, CrudAction.Update] },
            new Permission { Resource = Resource.Users, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] },
            new Permission { Resource = Resource.Items, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] },
            new Permission { Resource = Resource.Invoices, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] },
            new Permission { Resource = Resource.Customers, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] },
            new Permission { Resource = Resource.Plans, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete]  },
            new Permission { Resource = Resource.Reports, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] }
        ],
        [UserRole.ClientAdmin] =
        [
            new Permission { Resource = Resource.Clients, AllowedActions = [CrudAction.Read] },
            new Permission { Resource = Resource.Users, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] },
            new Permission { Resource = Resource.Items, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete] },
            new Permission { Resource = Resource.Invoices, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update] },
            new Permission { Resource = Resource.Customers, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update] },
            new Permission { Resource = Resource.Plans, AllowedActions = [CrudAction.Create, CrudAction.Read, CrudAction.Update] },
            new Permission { Resource = Resource.Reports, AllowedActions = [CrudAction.Create, CrudAction.Read] }
        ],
        [UserRole.Operator] =
        [
            new Permission { Resource = Resource.Clients, AllowedActions = [CrudAction.Read] },
            new Permission { Resource = Resource.Users, AllowedActions = [CrudAction.Read] },
            new Permission { Resource = Resource.Items, AllowedActions = [CrudAction.Read] },
            new Permission { Resource = Resource.Invoices, AllowedActions = [CrudAction.Create, CrudAction.Read] },
            new Permission { Resource = Resource.Customers, AllowedActions = [CrudAction.Create, CrudAction.Read] }
        ]
    };
}

