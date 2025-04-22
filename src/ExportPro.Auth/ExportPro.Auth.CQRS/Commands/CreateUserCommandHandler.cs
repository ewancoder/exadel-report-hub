using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Repositories;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Refit;

namespace ExportPro.Auth.CQRS.Commands;

public record CreateUserCommand(string Username, string Email, string Password, Guid? ClientId, UserRole? UserRole) : ICommand<Guid>, IPermissionedRequest
{
    public List<Guid>? ClientIds => ClientId.HasValue ? [ClientId.Value] : null;

    public Resource Resource => Resource.Users;

    public CrudAction Action => CrudAction.Create;
}

public class CreateUserCommandHandler(IUserRepository userRepository, IACLSharedApi aclApi)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<BaseResponse<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
       
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.None
        };
        var createdUser = await userRepository.AddOneAsync(user, cancellationToken);
        if (request.ClientId != null && request.UserRole != null)
        {
            await aclApi.GrantPermissionAsync(createdUser.Id.ToGuid(), (Guid)request.ClientId, (UserRole)request.UserRole);
        }

        return new SuccessResponse<Guid>
        (
            user.Id.ToGuid(),
            "User Created Successfully"
        );
    }
}

