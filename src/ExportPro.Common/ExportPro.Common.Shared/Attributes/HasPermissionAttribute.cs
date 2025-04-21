using ExportPro.Common.Shared.Enums;

namespace ExportPro.Common.Shared.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute(Resource resource, CrudAction action) : Attribute
{
    public Resource Resource { get; } = resource;
    public CrudAction Action { get; } = action;
}
