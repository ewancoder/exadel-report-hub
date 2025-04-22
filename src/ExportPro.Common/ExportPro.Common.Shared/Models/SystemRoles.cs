namespace ExportPro.Common.Shared.Models;

public static class SystemRoles
{
    public static readonly Guid SuperAdmin = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid Owner = Guid.Parse("00000000-0000-0000-0000-000000000002");
    public static readonly Guid ClientAdmin = Guid.Parse("00000000-0000-0000-0000-000000000003");
    public static readonly Guid Operator = Guid.Parse("00000000-0000-0000-0000-000000000004");
}

