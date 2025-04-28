using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.CommandHandlers.CustomerCommands;

public sealed class UpdateCustomerCommand : ICommand<Customer>
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? CountryId { get; set; }
}
public sealed class UpdateCustomerCommandHandler(ICustomerRepository repository)
    : ICommandHandler<UpdateCustomerCommand, Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id) || !ObjectId.TryParse(request.Id, out var objectId))
        {
            return new BaseResponse<Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Invalid customer ID format."]
            };
        }

        if (!string.IsNullOrEmpty(request.CountryId) && !ObjectId.TryParse(request.CountryId, out _))
        {
            return new BaseResponse<Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Invalid CountryId format."]
            };
        }

        var existingCustomer = await _repository.GetByIdAsync(objectId, cancellationToken);
        
        if (existingCustomer is null || existingCustomer.IsDeleted)
        {
            return new BaseResponse<Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Customer not found."]
            };
        }

        if (!string.IsNullOrEmpty(request.Name))
            existingCustomer.Name = request.Name.Trim();

        if (!string.IsNullOrEmpty(request.Email))
            existingCustomer.Email = request.Email.Trim();

        if (!string.IsNullOrEmpty(request.CountryId))
            existingCustomer.CountryId = request.CountryId;

        existingCustomer.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOneAsync(existingCustomer, cancellationToken);

        return new BaseResponse<Customer>
        {
            Data = existingCustomer,
            IsSuccess = true,
            ApiState = HttpStatusCode.OK
        };
    }
}