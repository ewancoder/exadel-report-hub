
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Models;

namespace ExportPro.Common.Shared.Helpers;

public static class RolePermissions
{
    public static readonly Dictionary<Role, List<Permission>> Matrix = new()
    {
        [Role.SuperAdmin] = new()
        {
            new Permission { Resource = Resource.Clients, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Users, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } }
        },
        [Role.Owner] = new()
        {
            new Permission { Resource = Resource.Clients, AllowedActions = new() { CrudAction.Read, CrudAction.Update } },
            new Permission { Resource = Resource.Users, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Items, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Invoices, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Customers, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Plans, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Reports, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } }
        },
        [Role.ClientAdmin] = new()
        {
            new Permission { Resource = Resource.Clients, AllowedActions = new() { CrudAction.Read } },
            new Permission { Resource = Resource.Users, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Items, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update, CrudAction.Delete } },
            new Permission { Resource = Resource.Invoices, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update } },
            new Permission { Resource = Resource.Customers, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update } },
            new Permission { Resource = Resource.Plans, AllowedActions = new() { CrudAction.Create, CrudAction.Read, CrudAction.Update } },
            new Permission { Resource = Resource.Reports, AllowedActions = new() { CrudAction.Create, CrudAction.Read } }
        },
        [Role.Operator] = new()
        {
            new Permission { Resource = Resource.Clients, AllowedActions = new() { CrudAction.Read } },
            new Permission { Resource = Resource.Users, AllowedActions = new() {  CrudAction.Read } },
            new Permission { Resource = Resource.Items, AllowedActions = new() {  CrudAction.Read } },
            new Permission { Resource = Resource.Invoices, AllowedActions = new() { CrudAction.Create, CrudAction.Read} },
            new Permission { Resource = Resource.Customers, AllowedActions = new() { CrudAction.Create, CrudAction.Read  } }
        }
    };
}

