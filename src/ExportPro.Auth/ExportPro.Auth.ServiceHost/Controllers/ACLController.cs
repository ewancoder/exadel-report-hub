using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.CQRS.Queries;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Library;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Auth.ServiceHost.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    public class ACLController(IMediator mediator) : ControllerBase
    {

        [HttpPost("check")]
        public  Task<BaseResponse<bool>> CheckPermission([FromBody] CheckPermissionRequest request) =>
             mediator.Send(new HasPermissionQuery(request.UserId, request.ClientId, request.Resource, request.Action));

        [HttpGet("{userId}/{clientId}")]
        public Task<BaseResponse<List<PermissionDTO>>> GetPermissions([FromRoute]string userId, [FromRoute] string clientId) =>
             mediator.Send(new GetUserClientPermissionsQuery(userId, clientId));
        
        [HttpPost("grant")]
        public Task<BaseResponse<bool>> GrantPermission([FromBody] GrantUserRoleCommand command) =>
           mediator.Send(command);
 

        [HttpPost("revoke")]
        public Task<BaseResponse<bool>> RevokePermission([FromBody] RemovePermissionCommand command) => 
            mediator.Send(command);

        [HttpPost("update-role")]
        public  Task<BaseResponse<bool>> UpdateUserRole([FromBody] UpdateUserRoleCommand command) 
            => mediator.Send(command);

        [HttpGet("user-clients")]
        public Task<BaseResponse<List<Guid>>> UserClients()
            => mediator.Send(new GetUserClientsQuery());

    }
}
