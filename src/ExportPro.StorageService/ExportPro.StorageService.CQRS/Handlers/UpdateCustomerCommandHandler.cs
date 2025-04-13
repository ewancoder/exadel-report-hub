using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Commands;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers;

public class UpdateCustomerCommandHandler(ICustomerRepository repository) : ICommandHandler<UpdateCustomerCommand, Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Customer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer is null || customer.IsDeleted)
        {
            return new BaseResponse<Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Customer not found." }
            };
        }

        customer.Name = request.Name;
        customer.Email = request.Email;
        customer.Country = request.Country;
        customer.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateOneAsync(customer, cancellationToken);

        return new BaseResponse<Customer> { Data = customer };
    }
}