using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed record CreateCustomerCommand(CreateUpdateCustomerDto CustomerDto) : ICommand<CustomerResponse>;

public sealed class CreateCustomerCommandHandler(ICustomerRepository repository, IMapper mapper)
    : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<BaseResponse<CustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var customer = mapper.Map<Customer>(request.CustomerDto);
        await repository.AddOneAsync(customer, cancellationToken);
        return new SuccessResponse<CustomerResponse>(mapper.Map<CustomerResponse>(customer));
    }
}
