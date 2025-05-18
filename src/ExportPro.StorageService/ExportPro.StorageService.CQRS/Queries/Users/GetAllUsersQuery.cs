using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace ExportPro.Auth.CQRS.Queries;

public record GetAllUsersQuery : IQuery<List<UserDto>>, IPermissionedRequest
{
    public List<Guid>? ClientIds => [];
    public Resource Resource => Resource.Users;
    public CrudAction Action => CrudAction.Read;
};

public class GetAllUsersQueryHandler(
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository,
    IACLService aclService
) : IQueryHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<BaseResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null)
            return new BadRequestResponse<List<UserDto>>("User context not found.");

        var role = TokenHelper.GetUserRole(user);

        var users = await userRepository.GetUsersAsync(cancellationToken);
        if (users == null)
            return new BadRequestResponse<List<UserDto>>("Users not found.");

        if (role == Common.Shared.Enums.Role.None)
        {
            var userId = TokenHelper.GetUserId(user);
            var userRoles = await aclService.GetAccessibleUserRolesAsync(userId, cancellationToken);
            if (userRoles == null || userRoles.Count == 0)
                return new BadRequestResponse<List<UserDto>>("User roles not found.");

            var accessibleUserIds = userRoles.Select(r => r.UserId).Distinct().ToHashSet();

            users = [.. users.Where(u => accessibleUserIds.Contains(u.Id))];
        }

        var userDtos = new List<UserDto>();

        foreach (var u in users)
        {
            var clientRoles = await aclService.GetAccessibleUserRolesAsync(u.Id, cancellationToken);
            var clientRoleDtos =
                clientRoles
                    ?.Select(cr => new UserClientRolesDTO
                    {
                        ClientId = cr.ClientId.ToGuid(),
                        Role = cr.Role,
                        UserId = cr.UserId.ToGuid(),
                    })
                    .ToList() ?? [];

            userDtos.Add(
                new UserDto
                {
                    Id = u.Id.ToGuid(),
                    Username = u.Username,
                    Email = u.Email,
                    ClientRoles = clientRoleDtos,
                }
            );
        }

        return new SuccessResponse<List<UserDto>>(userDtos);
    }
}
