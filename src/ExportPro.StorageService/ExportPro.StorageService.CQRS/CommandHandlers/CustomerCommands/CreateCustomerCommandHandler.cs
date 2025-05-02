using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed class CreateCustomerCommand : ICommand<CustomerResponse>
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required Guid CountryId { get; set; }
}

public sealed class CreateCustomerCommandHandler(ICustomerRepository repository, IMapper mapper)
    : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<BaseResponse<CustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var customer = new Customer
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Email = request.Email,
            CountryId = request.CountryId.ToObjectId(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            IsDeleted = false,
        };
        await repository.AddOneAsync(customer, cancellationToken);
        return new SuccessResponse<CustomerResponse>(mapper.Map<CustomerResponse>(customer));
    }
}
