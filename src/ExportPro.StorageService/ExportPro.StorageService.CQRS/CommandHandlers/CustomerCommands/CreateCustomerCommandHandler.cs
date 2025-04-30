using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public class CreateCustomerCommand : ICommand<CustomerResponse>
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required Guid CountryId { get; set; }
}

public class CreateCustomerCommandHandler(
    ICustomerRepository repository,
    IMapper mapper,
    IValidator<CreateCustomerCommand> validator
) : ICommandHandler<CreateCustomerCommand, CustomerResponse>
{
    public async Task<BaseResponse<CustomerResponse>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var countryIdString = request.CountryId.ToString();
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
        return new BaseResponse<CustomerResponse> { Data = mapper.Map<CustomerResponse>(customer) };
    }
}
