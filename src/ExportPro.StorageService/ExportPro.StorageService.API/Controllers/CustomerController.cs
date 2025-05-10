using ExportPro.Common.Shared.Attributes;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;
using ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExportPro.StorageService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [HasPermission(Resource.Customers, CrudAction.Create)]
    public Task<BaseResponse<CustomerResponse>> Create(
        [FromBody] CreateUpdateCustomerDto customerDto,
        CancellationToken cancellationToken
    ) => mediator.Send(new CreateCustomerCommand(customerDto), cancellationToken);

    [HttpPut("{id}")]
    [HasPermission(Resource.Customers, CrudAction.Update)]
    public Task<BaseResponse<CustomerResponse>> Update(
        [FromRoute] Guid id,
        [FromBody] CreateUpdateCustomerDto customerDto,
        CancellationToken cancellationToken
    ) => mediator.Send(new UpdateCustomerCommand(id, customerDto), cancellationToken);

    
    [HttpDelete("{id}")]
    [HasPermission(Resource.Customers, CrudAction.Delete)]
    public Task<BaseResponse<bool>> Delete([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new DeleteCustomerCommand(id), cancellationToken);

    [HttpGet("{id}")]
    [HasPermission(Resource.Customers, CrudAction.Read)]
    public Task<BaseResponse<CustomerDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken) =>
        mediator.Send(new GetCustomerByIdQuery(id.ToObjectId()), cancellationToken);

    [HasPermission(Resource.Customers, CrudAction.Read)]
    [HttpGet]
    public Task<BaseResponse<PaginatedListDto<CustomerDto>>> GetAll(
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    ) => mediator.Send(new GetPaginatedCustomersQuery(pageNumber, pageSize), cancellationToken);
}
