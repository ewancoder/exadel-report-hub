using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.CQRS.Queries;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExportPro.Auth.ServiceHost.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    public class ACLController(IMediator mediator) : ControllerBase
    {

        [HttpPost("check")]
        public  Task<BaseResponse<bool>> CheckPermission([FromBody] CheckPermissionRequest request) =>
             mediator.Send(new HasPermissionQuery((Guid)request.UserId, request.ClientId, request.Resource, request.Action));

        [HttpGet("{userId}/{clientId}")]
        public Task<BaseResponse<List<PermissionDTO>>> GetPermissions([FromRoute]Guid userId, [FromRoute] Guid clientId) =>
             mediator.Send(new GetUserClientPermissionsQuery(userId.ToObjectId(), clientId.ToObjectId()));
        
        [HttpPost("grant/{userId}/{clientId}/{role}")]
        public Task<BaseResponse<bool>> GrantPermission([FromRoute] Guid clientId, [FromRoute] Guid userId, [FromRoute] UserRole role) =>
        mediator.Send(new GrantUserRoleCommand(userId.ToObjectId(), clientId.ToObjectId(), role));
 

        [HttpPost("revoke/{userId}/{clientId}")]
        public Task<BaseResponse<bool>> RevokePermission([FromRoute] Guid clientId, [FromRoute]Guid userId) => 
            mediator.Send(new RemovePermissionCommand(userId.ToObjectId(),clientId.ToObjectId()));

        [HttpPost("update-role/{userId}/{clientId}/{role}")]
        public  Task<BaseResponse<bool>> UpdateUserRole([FromRoute] Guid clientId, [FromRoute] Guid userId, [FromRoute] UserRole role) 
            => mediator.Send(new UpdateUserRoleCommand(userId.ToObjectId(), clientId.ToObjectId(), role));

        [HttpGet("user-clients")]
        public Task<BaseResponse<List<Guid>>> UserClients()
            => mediator.Send(new GetUserClientsQuery());

        [HttpGet("user-roles")]
        public Task<BaseResponse<List<UserClientRolesDTO>>> UserRoles()
            => mediator.Send(new GetUserClientRolesQuery());

        [HttpDelete("{userId}")]
        public Task<BaseResponse<bool>> DeletePermission([FromRoute] Guid userId) =>
            mediator.Send(new DeleteUserClientRole(userId.ToObjectId()));

    }
}
