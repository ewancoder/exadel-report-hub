using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Refit;
namespace ExportPro.Auth.CQRS.Commands;

public record DeleteUserCommand(Guid Id) : ICommand<bool>;

public class DeleteUserCommandHandler(IUserRepository userRepository, IACLService aclService)
    : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id.ToObjectId(), cancellationToken);
        if (user == null)
            return new NotFoundResponse<bool>("User not found");
        await aclService.DeleteAllRoles(request.Id.ToObjectId(), cancellationToken);
        await userRepository.DeleteAsync(request.Id.ToObjectId(), cancellationToken);
        return new SuccessResponse<bool>(true, "User deleted successfully");
    }
}

