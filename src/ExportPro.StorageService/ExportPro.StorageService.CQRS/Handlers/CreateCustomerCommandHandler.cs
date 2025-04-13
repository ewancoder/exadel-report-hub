using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers;

public class CreateCustomerCommandHandler(ICustomerRepository repository) : ICommandHandler<CreateCustomerCommand, Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Email = request.Email,
            Country = request.Country,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddOneAsync(customer, cancellationToken);

        return new BaseResponse<Customer> { Data = customer };
    }
}