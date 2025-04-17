using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CustomerQueries;
using ExportPro.StorageService.DataAccess.Interfaces;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class GetAllCustomersQueryHandler(ICustomerRepository repository) : IQueryHandler<GetAllCustomersQuery, List<Models.Models.Customer>>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<List<Models.Models.Customer>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repository.GetAllAsync(cancellationToken);
        return new BaseResponse<List<Models.Models.Customer>> { Data = customers };
    }
}