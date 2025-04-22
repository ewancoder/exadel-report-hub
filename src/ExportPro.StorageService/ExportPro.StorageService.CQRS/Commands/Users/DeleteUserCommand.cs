using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using MongoDB.Bson;


namespace ExportPro.StorageService.CQRS.Commands.Users;

public record DeleteUserCommand(ObjectId Id) : ICommand<bool>;

public class DeleteUserCommandHandler(UserRepository userRepository)
    : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<BaseResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
            return new NotFoundResponse<bool>("User not found");

        await userRepository.DeleteAsync(request.Id, cancellationToken);
        return new SuccessResponse<bool>(true, "User deleted successfully");
    }
}

