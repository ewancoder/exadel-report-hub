using ExportPro.Auth.SDK.Models;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.StorageService.CQRS.Queries.Users;
public record GetAllUsersQuery : IQuery<List<UserDto>>;

public class GetAllUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<BaseResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllUsersAsync(cancellationToken);

        var dtos = users.Select(user => new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = RoleMappings.GuidToRole.GetValueOrDefault(Guid.Parse(user?.RoleId))
        }).ToList();

        return new SuccessResponse<List<UserDto>>(dtos);
    }
}

