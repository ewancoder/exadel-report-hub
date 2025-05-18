using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using MongoDB.Bson;


namespace ExportPro.Auth.CQRS.Commands;

public record UpdateUserCommand(ObjectId Id, ObjectId? ClientId, string Username, string Email, UserRole? Role, string Password) : ICommand<Guid>;

public class UpdateUserCommandHandler(IUserRepository userRepository, IACLService aCLService)
    : ICommandHandler<UpdateUserCommand, Guid>
{
    public async Task<BaseResponse<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingUser == null)
            return new NotFoundResponse<Guid>("User not found");


        existingUser.Username = request.Username;
        existingUser.Email = request.Email;
        existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);


        await userRepository.UpdateOneAsync(existingUser, cancellationToken);

        if(request.ClientId != null || request.Role != null)
        {
            await aCLService.UpdateUserRole(request.Id, request.ClientId.Value, (UserRole)request.Role, cancellationToken);
        }


        return new SuccessResponse<Guid>(existingUser.Id.ToGuid(), "User updated successfully");
    }
}