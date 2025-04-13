using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers;

public class GetCustomerByIdQueryHandler(ICustomerRepository repository) : IQueryHandler<GetCustomerByIdQuery, Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Customer>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null || customer.IsDeleted)
        {
            return new BaseResponse<Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Customer not found." }
            };
        }

        return new BaseResponse<Customer> { Data = customer };
    }
}