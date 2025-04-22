using ExportPro.Auth.SDK.Models;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Commands.Users;

public record CreateUserCommand(string Username, string Email, string Password,Role Role) : ICommand<ObjectId>;

public class CreateUserCommandHandler(UserRepository userRepository)
    : ICommandHandler<CreateUserCommand, ObjectId>
{
    public async Task<BaseResponse<ObjectId>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (!RoleMappings.RoleToGuid.TryGetValue(request.Role, out var roleId))
            return new BadRequestResponse<ObjectId>("Invalid Role.");
        
        var user = new User
        {
            Id = ObjectId.GenerateNewId(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = roleId.ToString()
        };
        await userRepository.AddOneAsync(user, cancellationToken);
        
        return new SuccessResponse<ObjectId>
        (
            user.Id,
            "User Created Successfully"
        );
    }
}

