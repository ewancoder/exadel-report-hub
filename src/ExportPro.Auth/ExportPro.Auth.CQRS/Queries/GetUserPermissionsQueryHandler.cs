using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Queries;

public record GetUserPermissionsQuery(ObjectId UserId) : IQuery<List<PermissionDTO>>;

public class GetUserPermissionsQueryHandler(IACLService aclService)
    : IQueryHandler<GetUserPermissionsQuery, List<PermissionDTO>>
{
    public async Task<BaseResponse<List<PermissionDTO>>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var permissions = await aclService.GetPermissions(request.UserId, cancellationToken: cancellationToken);
        if (permissions == null || permissions.Count == 0)
            return new NotFoundResponse<List<PermissionDTO>>("No permissions found for the user.");
        return new SuccessResponse<List<PermissionDTO>>(permissions);
    }
}
