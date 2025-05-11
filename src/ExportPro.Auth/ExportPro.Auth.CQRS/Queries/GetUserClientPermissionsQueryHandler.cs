using ExportPro.Auth.SDK.DTOs;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;
using ExportPro.AuthService.Services;
using ExportPro.Auth.SDK.Models;

namespace ExportPro.Auth.CQRS.Queries;

public record GetUserClientPermissionsQuery(string UserId, string ClientId): IQuery<List<PermissionDTO>>;


public class GetUserClientPermissionsQueryHandler(IACLService aclService) : IQueryHandler<GetUserClientPermissionsQuery, List<PermissionDTO>>
{
    public async Task<BaseResponse<List<PermissionDTO>>> Handle(GetUserClientPermissionsQuery request, CancellationToken cancellationToken)
    {
        var objectUserId = new MongoDB.Bson.ObjectId(request.UserId);
        var objectClientId = new MongoDB.Bson.ObjectId(request.ClientId);
        var permissions = await aclService.GetPermissions(objectUserId, objectClientId, cancellationToken: cancellationToken);
        if (permissions == null || permissions.Count == 0)
        {
            return new NotFoundResponse<List<PermissionDTO>>("No permissions found for the user.");
        }
        return new SuccessResponse<List<PermissionDTO>>(permissions);
    }
}

