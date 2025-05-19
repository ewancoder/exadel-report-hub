using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Commands;

public record GrantUserRoleCommand(ObjectId UserId, ObjectId ClientId, UserRole Role)
    : ICommand<bool>,
        IPermissionedRequest
{
    public List<Guid>? ClientIds => [ClientId.ToGuid()];
    public Resource Resource => Resource.Users;
    public CrudAction Action => CrudAction.Create;
}

public class GrantUserRoleCommandHandler(IACLService aclService) : ICommandHandler<GrantUserRoleCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(GrantUserRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await aclService.GetPermissions(request.UserId, request.ClientId, cancellationToken);
        if (role.Any(r => r.ClientId == request.ClientId))
            return new BadRequestResponse<bool>("User already has role in client. Please update user instead");
        await aclService.GrantPermission(request.UserId, request.ClientId, request.Role, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}
