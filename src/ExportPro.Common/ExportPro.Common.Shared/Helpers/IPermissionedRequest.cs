using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;


namespace ExportPro.Common.Shared.Helpers;
public interface IPermissionedRequest
{
    List<Guid>? ClientIds { get; }
    Resource Resource { get; }
    CrudAction Action { get; }
}
