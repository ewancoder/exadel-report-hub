using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using MongoDB.Bson;


namespace ExportPro.StorageService.CQRS.Commands.Users;

public record UpdateUserCommand(ObjectId Id, string Username, string Email, Role Role) : ICommand<ObjectId>;

public class UpdateUserCommandHandler(UserRepository userRepository)
    : ICommandHandler<UpdateUserCommand, ObjectId>
{
    public async Task<BaseResponse<ObjectId>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingUser == null)
            return new NotFoundResponse<ObjectId>("User not found");

        if (!RoleMappings.RoleToGuid.TryGetValue(request.Role, out var roleId) || request.Role == Role.SuperAdmin)
            return new BadRequestResponse<ObjectId>("Invalid Role.");

        existingUser.Username = request.Username;
        existingUser.Email = request.Email;
        existingUser.RoleId = roleId.ToString();

        await userRepository.UpdateOneAsync(existingUser, cancellationToken);

        return new SuccessResponse<ObjectId>(existingUser.Id, "User updated successfully");
    }
}