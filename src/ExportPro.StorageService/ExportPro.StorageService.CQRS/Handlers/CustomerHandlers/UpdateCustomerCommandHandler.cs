using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands.CustomerCommand;
using ExportPro.StorageService.DataAccess.Interfaces;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class UpdateCustomerCommandHandler(ICustomerRepository repository) : ICommandHandler<UpdateCustomerCommand, Models.Models.Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Models.Models.Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null || customer.IsDeleted)
        {
            return new BaseResponse<Models.Models.Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Customer not found." }
            };
        }

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.CountryId = request.CountryId;
        customer.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOneAsync(customer, cancellationToken);

        return new BaseResponse<Models.Models.Customer> { Data = customer };
    }
}