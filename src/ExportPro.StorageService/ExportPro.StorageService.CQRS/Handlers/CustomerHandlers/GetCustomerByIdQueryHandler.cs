﻿using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.CustomerQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using System.Net;

namespace ExportPro.StorageService.CQRS.Handlers.CustomerHandlers;

public class GetCustomerByIdQueryHandler(ICustomerRepository repository, IMapper mapper)
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
                Messages = new() { "Customer not found." }
            };
        }

        var dto = _mapper.Map<CustomerDto>(customer);
        return new BaseResponse<CustomerDto> { Data = dto };
    }
}