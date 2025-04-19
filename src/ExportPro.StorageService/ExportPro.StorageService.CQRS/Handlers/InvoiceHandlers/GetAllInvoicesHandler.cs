using System.Net;
using System.Runtime.CompilerServices;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.CQRS.Queries.InvoiceQueries;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ZstdSharp.Unsafe;

namespace ExportPro.StorageService.CQRS.Handlers.InvoiceHandlers;

public class GetAllInvoicesHandler(IInvoiceRepository repository, IMapper mapper)
    : IQueryHandler<GetAllInvoicesQuery, PaginatedListDto<InvoiceDto>>
{
    private readonly IInvoiceRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<PaginatedListDto<InvoiceDto>>> Handle(
        GetAllInvoicesQuery request,
        CancellationToken cancellationToken
    )
    {
        if (request.PageNumber < 1)
        {
            return new BaseResponse<PaginatedListDto<InvoiceDto>>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = new List<string> { "Page number must be greater than zero." },
            };
        }

        if (request.PageSize < 1)
        {
            return new BaseResponse<PaginatedListDto<InvoiceDto>>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = new List<string> { "Page size must be greater than zero." },
            };
        }

        var parameters = new PaginationParameters { PageNumber = request.PageNumber, PageSize = request.PageSize };

        var paginatedInvoices = await _repository.GetAllPaginatedAsync(
            parameters,
            request.IncludeDeleted,
            cancellationToken
        );

        var invoiceDtos = paginatedInvoices
            .Items.Select(invoice => new InvoiceDto
            {
                Id = invoice.Id.ToString(),
                InvoiceNumber = invoice.InvoiceNumber,
                IssueDate = invoice.IssueDate,
                DueDate = invoice.DueDate,
                CurrencyId = invoice.CurrencyId,
                PaymentStatus = invoice.PaymentStatus,
                BankAccountNumber = invoice.BankAccountNumber,
                ClientId = invoice.ClientId,
                Items = invoice.Items.Select(_ => _mapper.Map<ItemDtoForClient>(_)).ToList(),
            })
            .ToList();

        var paginatedDto = new PaginatedListDto<InvoiceDto>(
            invoiceDtos,
            paginatedInvoices.TotalCount,
            paginatedInvoices.PageNumber,
            paginatedInvoices.TotalPages
        );

        return new BaseResponse<PaginatedListDto<InvoiceDto>>
        {
            Data = paginatedDto,
            IsSuccess = true,
            ApiState = HttpStatusCode.OK,
        };
    }
}
