using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerCommand command,
        CancellationToken cancellationToken
    ) => Ok(await mediator.Send(command, cancellationToken));

    [HttpPut("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] CreateUpdateCustomerDto customerDto,
        CancellationToken cancellationToken
    )
    {
        return Ok(await mediator.Send(new UpdateCustomerCommand(id, customerDto), cancellationToken));
    }

    [HttpDelete("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new DeleteCustomerCommand(id), cancellationToken));
    }

    [HttpGet("{id}")]
    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Read)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetCustomerByIdQuery(id.ToObjectId()), cancellationToken));
    }

    [HasPermission(Common.Shared.Enums.Resource.Customers, Common.Shared.Enums.CrudAction.Read)]
    [HttpGet]
    public async Task<ActionResult<BaseResponse<PaginatedList<Customer>>>> GetAll(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool includeDeleted = false
    )
    {
        var query = new GetPaginatedCustomersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IncludeDeleted = includeDeleted,
        };
        var response = await mediator.Send(query);
        return StatusCode((int)response.ApiState, response);
    }
}
