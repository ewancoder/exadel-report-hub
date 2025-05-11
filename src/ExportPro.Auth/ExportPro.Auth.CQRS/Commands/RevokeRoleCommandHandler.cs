using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;


namespace ExportPro.Auth.CQRS.Commands;
public record RemovePermissionCommand(ObjectId UserId, ObjectId ClientId) : ICommand<bool>;

public class RemovePermissionCommandHandler(IACLService aclService) : ICommandHandler<RemovePermissionCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(RemovePermissionCommand request, CancellationToken cancellationToken)
    {
        await aclService.RemovePermission(request.UserId, request.ClientId, cancellationToken);
        return new SuccessResponse<bool>(true); 
    }
}