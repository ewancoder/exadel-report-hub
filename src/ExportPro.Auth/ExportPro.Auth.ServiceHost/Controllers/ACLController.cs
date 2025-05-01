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
        public  Task<BaseResponse<bool>> CheckPermission([FromBody] HasPermissionQueryHandler request) =>
             mediator.Send(request);

        [HttpGet("{userId}/{clientId}")]
        public Task<BaseResponse<List<PermissionDTO>>> GetPermissions([FromRoute]string userId, [FromRoute] string clientId) =>
             mediator.Send(new GetUserClientPermissionsQuery(userId, clientId));
        
        [HttpPost("grant")]
        public async Task<IActionResult> GrantPermission([FromBody] GrantUserRoleCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("revoke")]
        public Task<BaseResponse<bool>> RevokePermission([FromBody] RemovePermissionCommand command) => 
            mediator.Send(command);

        [HttpPost("update-role")]
        public  Task<BaseResponse<bool>> UpdateUserRole([FromBody] UpdateUserRoleCommand command) 
            => mediator.Send(command);

    }
}
