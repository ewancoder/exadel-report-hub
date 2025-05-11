using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Commands;

public record GrantUserRoleCommand(string UserId, string ClientId, UserRole Role) : ICommand<bool>;

public class GrantUserRoleCommandHandler(IACLService aclService) : ICommandHandler<GrantUserRoleCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(GrantUserRoleCommand request, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.UserId, out var userId))
            return new BadRequestResponse<bool>("Invalid UserId");

        if (!ObjectId.TryParse(request.ClientId, out var clientId))
             return new BadRequestResponse<bool>("Invalid ClientId");

        await aclService.GrantPermission(userId, clientId, request.Role, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}
