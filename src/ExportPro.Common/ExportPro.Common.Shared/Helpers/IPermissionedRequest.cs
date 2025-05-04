using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;


namespace ExportPro.Common.Shared.Helpers;
public interface IPermissionedRequest
{
    List<string>? ClientIds { get; }
    Resource Resource { get; }
    CrudAction Action { get; }
}
