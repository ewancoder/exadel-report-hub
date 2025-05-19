using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;

namespace ExportPro.Auth.CQRS.Commands;

public record DeleteUserClientRole(ObjectId UserId) : ICommand<bool>, IPermissionedRequest
{
    public List<Guid>? ClientIds => null;
    public Resource Resource => Resource.Users;
    public CrudAction Action => CrudAction.Delete;
}

public class DeleteUserClientRolesCommandHandler(IUserRepository userRepository, IACLService aclService)
    : ICommandHandler<DeleteUserClientRole, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteUserClientRole request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetOneAsync(x => x.Id == request.UserId, cancellationToken);
        if (user == null)
            return new NotFoundResponse<bool>("User not found.");
        var clientRoles = await aclService.GetAccessibleUserRolesAsync(request.UserId, cancellationToken);
        if (clientRoles == null)
            return new NotFoundResponse<bool>("User roles not found.");
        await aclService.DeleteAllRoles(request.UserId, cancellationToken);
        return new SuccessResponse<bool>(true, "User roles deleted successfully.");
    }
}
