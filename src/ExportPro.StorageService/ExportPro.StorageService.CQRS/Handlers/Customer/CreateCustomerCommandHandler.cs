using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.Customer;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.Handlers.Customer;

public class CreateCustomerCommandHandler(ICustomerRepository repository) : ICommandHandler<CreateCustomerCommand, Models.Models.Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Models.Models.Customer>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Models.Models.Customer
        {
            Id = ObjectId.GenerateNewId(),
            Name = request.Name,
            Email = request.Email,
            CountryId = request.CountryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repository.AddOneAsync(customer, cancellationToken);

        return new BaseResponse<Models.Models.Customer> { Data = customer };
    }
}