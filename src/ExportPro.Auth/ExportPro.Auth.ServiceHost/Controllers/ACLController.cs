using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.CQRS.Queries;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Auth.ServiceHost.Controllers;

[ApiController]
[Authorize]
[Route("api/permissions")]
public class ACLController(IMediator mediator) : ControllerBase
{
    [HttpPost("check")]
    public Task<BaseResponse<bool>> CheckPermission([FromBody] CheckPermissionRequest request)
    {
        return mediator.Send(
            new HasPermissionQuery((Guid)request.UserId, request.ClientId, request.Resource, request.Action)
        );
    }

    [HttpGet("{userId}/{clientId}")]
    public Task<BaseResponse<List<PermissionDTO>>> GetPermissions([FromRoute] Guid userId, [FromRoute] Guid clientId)
    {
        return mediator.Send(new GetUserClientPermissionsQuery(userId.ToObjectId(), clientId.ToObjectId()));
    }

    [HttpPost("grant/{userId}/{clientId}/{role}")]
    public Task<BaseResponse<bool>> GrantPermission(
        [FromRoute] Guid clientId,
        [FromRoute] Guid userId,
        [FromRoute] UserRole role
    )
    {
        return mediator.Send(new GrantUserRoleCommand(userId.ToObjectId(), clientId.ToObjectId(), role));
    }

    [HttpPost("revoke/{userId}/{clientId}")]
    public Task<BaseResponse<bool>> RevokePermission([FromRoute] Guid clientId, [FromRoute] Guid userId)
    {
        return mediator.Send(new RemovePermissionCommand(userId.ToObjectId(), clientId.ToObjectId()));
    }

    [HttpPost("update-role/{userId}/{clientId}/{role}")]
    public Task<BaseResponse<bool>> UpdateUserRole(
        [FromRoute] Guid clientId,
        [FromRoute] Guid userId,
        [FromRoute] UserRole role
    )
    {
        return mediator.Send(new UpdateUserRoleCommand(userId.ToObjectId(), clientId.ToObjectId(), role));
    }

    [HttpGet("user-clients")]
    public Task<BaseResponse<List<Guid>>> UserClients()
    {
        return mediator.Send(new GetUserClientsQuery());
    }

    [HttpGet("user-roles")]
    public Task<BaseResponse<List<UserClientRolesDTO>>> UserRoles()
    {
        return mediator.Send(new GetUserClientRolesQuery());
    }

    [HttpDelete("{userId}")]
    public Task<BaseResponse<bool>> DeletePermission([FromRoute] Guid userId)
    {
        return mediator.Send(new DeleteUserClientRole(userId.ToObjectId()));
    }
}
