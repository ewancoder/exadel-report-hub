using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;


namespace ExportPro.Common.Shared.Helpers;
public interface IPermissionedRequest
{
    ObjectId? ClientId { get; }
    Resource Resource { get; }
    CrudAction Action { get; }
}
