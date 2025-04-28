using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Commands;
public record GrantUserRoleCommand(ObjectId UserId, ObjectId ClientId, UserRole Role) : ICommand<bool>;

public class GrantUserRoleCommandHandler(IACLService aclService) : ICommandHandler<GrantUserRoleCommand, bool>
{
    private readonly IACLService _aclService = aclService;

    public async Task<BaseResponse<bool>> Handle(GrantUserRoleCommand request, CancellationToken cancellationToken)
    {
        await _aclService.GrantPermission(request.UserId, request.ClientId, request.Role, cancellationToken);
        return new SuccessResponse<bool>(true);
    }
}
