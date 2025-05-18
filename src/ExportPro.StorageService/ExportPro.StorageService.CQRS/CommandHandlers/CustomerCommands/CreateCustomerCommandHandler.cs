using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed record CreateCustomerCommand(CreateUpdateCustomerDto CustomerDto) : ICommand<CustomerResponse>, IPermissionedRequest
{
    public List<Guid>? ClientIds { get; init; } = [];
    public Resource Resource { get; init; } = Resource.Customers;
    public CrudAction Action { get; init; } = CrudAction.Create;
};

public sealed class CreateCustomerCommandHandler(
    IHttpContextAccessor httpContext,
    ICustomerRepository repository,
    IMapper mapper
) : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<BaseResponse<CustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var customer = mapper.Map<Customer>(request.CustomerDto);
        customer.CreatedBy = httpContext.HttpContext?.User.FindFirst(ClaimTypes.Name)!.Value;
        await repository.AddOneAsync(customer, cancellationToken);
        return new SuccessResponse<CustomerResponse>(mapper.Map<CustomerResponse>(customer));
    }
}
