using ExportPro.Auth.SDK.DTOs;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;
using ExportPro.AuthService.Services;
using ExportPro.Auth.SDK.Models;

namespace ExportPro.Auth.CQRS.Queries;

public record GetUserClientPermissionsQuery(ObjectId UserId, ObjectId ClientId): IQuery<List<PermissionDTO>>;


public class GetUserClientPermissionsQueryHandler(IACLService aclService) : IQueryHandler<GetUserClientPermissionsQuery, List<PermissionDTO>>
{
    public async Task<BaseResponse<List<PermissionDTO>>> Handle(GetUserClientPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await aclService.GetPermissions(request.UserId, request.ClientId, cancellationToken: cancellationToken);
        if (permissions == null || permissions.Count == 0)
        {
            return new NotFoundResponse<List<PermissionDTO>>("No permissions found for the user.");
        }
        return new SuccessResponse<List<PermissionDTO>>(permissions);
    }
}

