using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Extensions;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.QueryHandlers.CustomerQueries;

public sealed class GetPaginatedCustomersQuery : IQuery<PaginatedListDto<CustomerDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool IncludeDeleted { get; set; } = false;
}

public sealed class GetPaginatedCustomersQueryHandler(ICustomerRepository repository)
    : IQueryHandler<GetPaginatedCustomersQuery, PaginatedListDto<CustomerDto>>
{
    public async Task<BaseResponse<PaginatedListDto<CustomerDto>>> Handle(
        GetPaginatedCustomersQuery request,
        CancellationToken cancellationToken
    )
    {
        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };

        var paginatedCustomers = await repository.GetAllPaginatedAsync(
            parameters,
            request.IncludeDeleted,
            cancellationToken
        );

        var customerDtos = paginatedCustomers
            .Items.Select(c => new CustomerDto
            {
                Id = c.Id.ToGuid(),
                Name = c.Name,
                Email = c.Email,
                CountryId = c.CountryId.ToGuid(),
                IsDeleted = c.IsDeleted,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
            })
            .ToList();

        var dto = new PaginatedListDto<CustomerDto>(
            customerDtos,
            paginatedCustomers.TotalCount,
            paginatedCustomers.PageNumber,
            paginatedCustomers.TotalPages
        );

        return new SuccessResponse<PaginatedListDto<CustomerDto>>(dto, "Successfully retrieved customers.");
    }
}
