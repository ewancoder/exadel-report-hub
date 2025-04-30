using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;

public sealed record GetCustomerByIdQuery(ObjectId Id) : IQuery<CustomerDto>;

public sealed class GetCustomerByIdQueryHandler(ICustomerRepository repository, IMapper mapper)
    : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
{
    public async Task<BaseResponse<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var customer = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null || customer.IsDeleted)
        {
            return new NotFoundResponse<CustomerDto>("Customer not found.");
        }

        var dto = mapper.Map<CustomerDto>(customer);
        return new SuccessResponse<CustomerDto>(dto, "Successfully retrieved customer.");
    }
}
