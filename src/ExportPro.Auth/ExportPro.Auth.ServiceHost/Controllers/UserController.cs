using ExportPro.Auth.CQRS.Commands;
using ExportPro.Auth.CQRS.Queries;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ExportPro.Auth.ServiceHost.Controllers;
[ApiController]
[Authorize]
[Route("api/user/")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user")]
    [ProducesResponseType(typeof(SuccessResponse<Guid>), 201)]
    public Task<BaseResponse<Guid>> CreateUser([FromBody] CreateUpdateUserDTO command) =>
         mediator.Send(new CreateUserCommand(command.Username, command.Email, command.Password, command.ClientId, command.Role));

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users")]
    [ProducesResponseType(typeof(SuccessResponse<List<UserDto>>), 200)]
    public Task<BaseResponse<List<UserDto>>> GetAllUsers() =>
        mediator.Send(new GetAllUsersQuery());

    [HttpGet("current")]
    [SwaggerOperation(Summary = "Get current user")]
    [ProducesResponseType(typeof(SuccessResponse<UserDto>), 200)]
    public Task<BaseResponse<UserDto>> GetCurrentUser() =>
        mediator.Send(new GetCurrentUserQuery());

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a user")]
    [ProducesResponseType(typeof(SuccessResponse<ObjectId>), 200)]
    public Task<BaseResponse<Guid>> UpdateUser([Required] Guid id, [FromBody] CreateUpdateUserDTO command) =>
        mediator.Send(new UpdateUserCommand(id.ToObjectId(), command.ClientId.Value.ToObjectId(), command.Username, command.Email, command.Role, command.Password));

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user")]
    [ProducesResponseType(typeof(SuccessResponse<bool>), 200)]
    public Task<BaseResponse<bool>> DeleteUser([Required] Guid id) =>
        mediator.Send(new DeleteUserCommand(id));
}

