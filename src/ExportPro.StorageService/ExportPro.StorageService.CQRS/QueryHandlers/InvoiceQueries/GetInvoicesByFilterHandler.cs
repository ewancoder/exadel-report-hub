using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.ModelFilters;
using ExportPro.StorageService.SDK.PaginationParams;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetInvoicesByFilter(InvoiceFilter Filters, Guid clienId) : IQuery<PaginatedList<InvoiceDto>>;

public sealed class GetInvoicesByFilterHandler(
    IInvoiceRepository repository,
    ICustomerRepository customerRepository,
    ICountryRepository countryRepository,
    ICurrencyRepository currencyRepository,
    IMapper mapper
) : IQueryHandler<GetInvoicesByFilter, PaginatedList<InvoiceDto>>
{
    public async Task<BaseResponse<PaginatedList<InvoiceDto>>> Handle(
        GetInvoicesByFilter request,
        CancellationToken cancellationToken
    )
    {
        var invoicesModel = await repository.GetAllByClientIdAsync(request.clienId.ToObjectId(), cancellationToken);
        var invoicesDto = mapper.Map<List<InvoiceDto>>(invoicesModel);
        var customer = request.Filters.Customer;
        if (customer!.Name is not null)
        {
            var customerId = await customerRepository.GetOneAsync(
                x => x.Name == customer.Name && !x.IsDeleted,
                cancellationToken
            );
            if (customerId != null)
                invoicesDto = [.. invoicesDto.Where(x => x.CustomerId.ToObjectId() == customerId.Id)];
        }

        if (customer.Address is not null)
        {
            var customerId = await customerRepository.GetOneAsync(
                x => x.Address == customer.Address && !x.IsDeleted,
                cancellationToken
            );
            if (customerId != null)
                invoicesDto = [.. invoicesDto.Where(x => x.CustomerId.ToObjectId() == customerId.Id)];
        }

        if (customer.Email is not null)
        {
            var customerId = await customerRepository.GetOneAsync(
                x => x.Email == customer.Email && !x.IsDeleted,
                cancellationToken
            );
            if (customerId != null)
                invoicesDto = [.. invoicesDto.Where(x => x.CustomerId.ToObjectId() == customerId.Id)];
        }

        if (customer.CountryId is not null)
        {
            var countryId = await countryRepository.GetOneAsync(
                x => x.Id.ToGuid() == customer.CountryId && !x.IsDeleted,
                cancellationToken
            );
            if (countryId != null)
            {
                var customerId = await customerRepository.GetOneAsync(
                    x => x.CountryId == countryId.Id && !x.IsDeleted,
                    cancellationToken
                );
                if (customerId != null)
                    invoicesDto = [.. invoicesDto.Where(x => x.CustomerId.ToObjectId() == customerId.Id)];
            }
        }

        var invoicesPaginated = invoicesDto.ToPaginatedList(request.Filters.PageNumber, request.Filters.PageSize);
        return new SuccessResponse<PaginatedList<InvoiceDto>>(invoicesPaginated);
    }
}
