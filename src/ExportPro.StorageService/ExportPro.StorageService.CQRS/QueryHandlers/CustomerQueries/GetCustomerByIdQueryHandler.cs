using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using MongoDB.Bson;
using System.Net;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;

public record GetCustomerByIdQuery(ObjectId Id) : IQuery<CustomerDto>;

public sealed class GetCustomerByIdQueryHandler(ICustomerRepository repository, IMapper mapper)
    : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
{
    private readonly ICustomerRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null || customer.IsDeleted)
        {
            return new BaseResponse<CustomerDto>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Customer not found."]
            };
        }

        var dto = _mapper.Map<CustomerDto>(customer);
        return new BaseResponse<CustomerDto> { Data = dto };
    }
}