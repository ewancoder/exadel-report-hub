using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.Customer;
using ExportPro.StorageService.DataAccess.Interfaces;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.Customer;

public class GetCustomerByIdQueryHandler(ICustomerRepository repository) : IQueryHandler<GetCustomerByIdQuery, Models.Models.Customer>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<Models.Models.Customer>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null || customer.IsDeleted)
        {
            return new BaseResponse<Models.Models.Customer>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = new() { "Customer not found." }
            };
        }

        return new BaseResponse<Models.Models.Customer> { Data = customer };
    }
}