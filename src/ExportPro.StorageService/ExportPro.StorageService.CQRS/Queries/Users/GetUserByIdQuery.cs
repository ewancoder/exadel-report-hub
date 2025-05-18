using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Queries.Users;

public record GetUserByIdQuery(ObjectId Id) : IQuery<UserDto>;

public class GetUserByIdQueryHandler(UserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<BaseResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
            return new NotFoundResponse<UserDto>("User not found");

        var role = RoleMappings.GuidToRole.GetValueOrDefault(Guid.Parse(user?.RoleId), default);

        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = role
        };

        return new SuccessResponse<UserDto>(dto);
    }
}



