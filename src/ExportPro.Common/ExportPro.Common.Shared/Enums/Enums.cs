

namespace ExportPro.Common.Shared.Enums;

public enum UserRole
{
    Owner,
    ClientAdmin,
    Operator
}

public enum CrudAction
{
    Create,
    Read,
    Update,
    Delete
}

public enum Resource
{
    Clients,
    Users,
    Items,
    Invoices,
    Customers,
    Plans,
    Reports
}

public enum Role
{
    None,
    SuperAdmin
}

