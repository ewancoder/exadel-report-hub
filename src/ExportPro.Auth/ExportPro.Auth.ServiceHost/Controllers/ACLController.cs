using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.CQRS.Queries;
using ExportPro.AuthService.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.Auth.ServiceHost.Controllers
{
    [ApiController]
    [Route("api/permissions")]
    public class ACLController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("check")]
        public async Task<IActionResult> CheckPermission([FromBody] HasPermissionQuery request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("{userId}/{clientId}")]
        public async Task<IActionResult> GetPermissions([FromRoute]string userId, [FromRoute] string clientId)
        {
            var objectUserId = new MongoDB.Bson.ObjectId(userId);
            var objectClientId = new MongoDB.Bson.ObjectId(clientId);
            var permissions = _mediator.Send(new GetUserClientPermissionsQuery(objectUserId, objectClientId));
            return Ok(permissions);
        }

        [HttpPost("grant")]
        public async Task<IActionResult> GrantPermission([FromBody] GrantUserRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokePermission([FromBody] RemovePermissionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("update-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
