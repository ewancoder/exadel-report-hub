using System.Net;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using MongoDB.Driver;

namespace ExportPro.StorageService.CQRS.QueryHandlers.InvoiceQueries;

public sealed record GetTotalInvoicesQuery(TotalInvoicesDto InvoicesDto) : IQuery<long>;

public sealed class GetTotalInvoicesHandler(IInvoiceRepository invoiceRepository)
    : IQueryHandler<GetTotalInvoicesQuery, long>
{
    public async Task<BaseResponse<long>> Handle(GetTotalInvoicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var filter =
                Builders<Invoice>.Filter.Gte(x => x.IssueDate, request.InvoicesDto.StartDate)
                & Builders<Invoice>.Filter.Lte(x => x.IssueDate, request.InvoicesDto.EndDate);

            if (request.InvoicesDto.ClientId != Guid.Empty)
            {
                filter &= Builders<Invoice>.Filter.Eq(x => x.ClientId, request.InvoicesDto.ClientId.ToObjectId());
            }

            var count = await invoiceRepository.CountAsync(filter, cancellationToken);

            if (count == 0)
            {
                return new BaseResponse<long>
                {
                    Data = 0,
                    IsSuccess = true,
                    ApiState = HttpStatusCode.OK,
                    Messages = ["No invoices issued in selected period."],
                };
            }

            return new BaseResponse<long>
            {
                Data = count,
                IsSuccess = true,
                ApiState = HttpStatusCode.OK,
                Messages = ["Total invoices fetched successfully."],
            };
        }
        catch (Exception ex)
        {
            return new BaseResponse<long>
            {
                Data = 0,
                IsSuccess = false,
                ApiState = HttpStatusCode.InternalServerError,
                Messages = [$"Error counting invoices: {ex.Message}"],
            };
        }
    }
}
