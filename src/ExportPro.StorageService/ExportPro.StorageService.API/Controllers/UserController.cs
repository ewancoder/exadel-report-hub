using ExportPro.Common.Shared.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.Commands.Users;
using ExportPro.StorageService.CQRS.Queries.Users;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.UserService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/user/")]
public class UserController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new user")]
    [ProducesResponseType(typeof(SuccessResponse<ObjectId>), 201)]
    [HasPermission(Resource.Users, CrudAction.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var response = await mediator.Send(command);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users")]
    [ProducesResponseType(typeof(SuccessResponse<List<UserDto>>), 200)]
    [HasPermission(Resource.Users, CrudAction.Read)]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await mediator.Send(new GetAllUsersQuery());
        return StatusCode((int)response.ApiState, response);
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [ProducesResponseType(typeof(SuccessResponse<UserDto>), 200)]
    [HasPermission(Resource.Users, CrudAction.Read)]
    public async Task<IActionResult> GetById([Required] string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(new BadRequestResponse<UserDto>("Invalid ID format"));

        var response = await mediator.Send(new GetUserByIdQuery(objectId));
        return StatusCode((int)response.ApiState, response);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a user")]
    [ProducesResponseType(typeof(SuccessResponse<ObjectId>), 200)]
    [HasPermission(Resource.Users, CrudAction.Update)]
    public async Task<IActionResult> UpdateUser([Required] string id, [FromBody] UpdateUserCommand command)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(new BadRequestResponse<ObjectId>("Invalid ID format"));

        var finalCommand = command with { Id = objectId };
        var response = await mediator.Send(finalCommand);
        return StatusCode((int)response.ApiState, response);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user")]
    [ProducesResponseType(typeof(SuccessResponse<bool>), 200)]
    [HasPermission(Resource.Users, CrudAction.Delete)]
    public async Task<IActionResult> DeleteUser([Required] string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(new BadRequestResponse<bool>("Invalid ID format"));

        var response = await mediator.Send(new DeleteUserCommand(objectId));
        return StatusCode((int)response.ApiState, response);
    }
}
