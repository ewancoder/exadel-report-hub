using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.CQRS.Handlers;

public class GetAllCustomersQueryHandler : IQueryHandler<GetAllCustomersQuery, List<Customer>>
{
    private readonly ICustomerRepository _repository;

    public GetAllCustomersQueryHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<BaseResponse<List<Customer>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Customer>> { Data = customers };
    }
}