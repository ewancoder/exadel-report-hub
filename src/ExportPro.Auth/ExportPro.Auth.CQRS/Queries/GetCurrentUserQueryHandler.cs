using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace ExportPro.Auth.CQRS.Queries;

public record GetCurrentUserQuery : IQuery<UserDto>;
public sealed class GetCurrentUserQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IACLService aclService) : IQueryHandler<GetCurrentUserQuery, UserDto>
{
    public async Task<BaseResponse<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null)
            return new BadRequestResponse<UserDto>("User context not found.");

        var userId = TokenHelper.GetUserId(user);

        var dbUser = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (dbUser == null)
            return new NotFoundResponse<UserDto>("User not found.");

        var clientRoles = await aclService.GetAccessibleUserRolesAsync(dbUser.Id, cancellationToken);
        var clientRoleDtos = clientRoles?
            .Select(cr => new UserClientRolesDTO
            {
                ClientId = cr.ClientId.ToGuid(),
                Role = cr.Role,
                UserId = cr.UserId.ToGuid()
            }).ToList() ?? [];

        var userDto = new UserDto
        {
            Id = dbUser.Id.ToGuid(),
            Username = dbUser.Username,
            Email = dbUser.Email,
            ClientRoles = clientRoleDtos,
            Role = TokenHelper.GetUserRole(user),
        };
        return new SuccessResponse<UserDto>(userDto);
    }
}

