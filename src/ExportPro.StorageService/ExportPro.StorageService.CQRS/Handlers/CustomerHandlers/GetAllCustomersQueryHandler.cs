using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CustomerQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class GetAllCustomersQueryHandler(ICustomerRepository repository, IMapper mapper)
    : IQueryHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _repository.GetAllAsync(cancellationToken);
        var mapped = _mapper.Map<List<CustomerDto>>(customers);
        return new BaseResponse<List<CustomerDto>> { Data = mapped };
    }
}