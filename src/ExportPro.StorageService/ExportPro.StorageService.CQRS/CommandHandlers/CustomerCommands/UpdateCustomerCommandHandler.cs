using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed record UpdateCustomerCommand(Guid Id, CreateUpdateCustomerDto Customer) : ICommand<CustomerResponse>;

public sealed class UpdateCustomerCommandHandler(ICustomerRepository repository, IMapper mapper)
    : ICommandHandler<UpdateCustomerCommand, CustomerResponse>
{
    public async Task<BaseResponse<CustomerResponse>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingCustomer = await repository.GetOneAsync(
            x => x.Id == request.Id.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (existingCustomer is null || existingCustomer.IsDeleted)
            return new NotFoundResponse<CustomerResponse>("Customer not found.");
        if (!string.IsNullOrEmpty(request.Customer.Name))
            existingCustomer.Name = request.Customer.Name.Trim();

        if (!string.IsNullOrEmpty(request.Customer.Email))
            existingCustomer.Email = request.Customer.Email.Trim();

        if (request.Customer.CountryId != Guid.Empty)
            existingCustomer.CountryId = request.Customer.CountryId.ToObjectId();

        existingCustomer.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateOneAsync(existingCustomer, cancellationToken);
        var customerResp = mapper.Map<CustomerResponse>(existingCustomer);
        return new SuccessResponse<CustomerResponse>(customerResp, "The customer Updated successfully.");
    }
}
