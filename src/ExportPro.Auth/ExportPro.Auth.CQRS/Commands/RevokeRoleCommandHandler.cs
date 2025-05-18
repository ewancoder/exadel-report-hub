using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Commands;

public record RemovePermissionCommand(ObjectId UserId, ObjectId ClientId) : ICommand<bool>, IPermissionedRequest
{
    public List<Guid>? ClientIds => [ClientId.ToGuid()];
    public Resource Resource => Resource.Users;
    public CrudAction Action => CrudAction.Delete;
}

public class RemovePermissionCommandHandler(IACLService aclService) : ICommandHandler<RemovePermissionCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(RemovePermissionCommand request, CancellationToken cancellationToken)
    {
        await aclService.RemovePermission(request.UserId, request.ClientId, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}
