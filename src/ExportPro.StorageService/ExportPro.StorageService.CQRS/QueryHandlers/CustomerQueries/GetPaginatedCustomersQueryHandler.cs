using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTOs;
using ExportPro.StorageService.SDK.PaginationParams;
using System.Net;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;

public class GetPaginatedCustomersQuery : IQuery<PaginatedListDto<CustomerDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; } = false;
}
public class GetPaginatedCustomersQueryHandler(ICustomerRepository repository) : IQueryHandler<GetPaginatedCustomersQuery, PaginatedListDto<CustomerDto>>
{
    private readonly ICustomerRepository _repository = repository;

    public async Task<BaseResponse<PaginatedListDto<CustomerDto>>> Handle(
        GetPaginatedCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var paginatedCustomers = await _repository.GetAllPaginatedAsync(parameters, request.IncludeDeleted, cancellationToken);

        var customerDtos = paginatedCustomers.Items.Select(c => new CustomerDto
        {
            Id = c.Id.ToString(),
            Name = c.Name,
            Email = c.Email,
            CountryId = c.CountryId,
            IsDeleted = c.IsDeleted,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        var dto = new PaginatedListDto<CustomerDto>(
            customerDtos,
            paginatedCustomers.TotalCount,
            paginatedCustomers.PageNumber,
            paginatedCustomers.TotalPages
            );

        return new BaseResponse<PaginatedListDto<CustomerDto>>
        {
            Data = dto,
            IsSuccess = true,
            ApiState = HttpStatusCode.OK
        };
    }
}
