using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ItemQueries;

public record GetItemsQuery(Guid ClientId) : IQuery<List<ItemResponse>>;

public class GetItemsQueryHandler(
    IClientRepository clientRepository,
    IInvoiceRepository invoiceRepository,
    IMapper mapper
) : IQueryHandler<GetItemsQuery, List<ItemResponse>>
{
    public async Task<BaseResponse<List<ItemResponse>>> Handle(
        GetItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        // First check if client exists
        var client = await clientRepository.GetOneAsync(
            c => c.Id == request.ClientId.ToObjectId() && !c.IsDeleted,
            cancellationToken
        );

        if (client == null)
            return new BaseResponse<List<ItemResponse>>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Client not found."],
            };

        // Get all invoices for this client
        var parameters = new ExportPro.StorageService.SDK.PaginationParams.PaginationParameters
        {
            PageNumber = 1,
            PageSize = 1000, // Large enough to get all invoices
        };

        var paginatedInvoices = await invoiceRepository.GetAllPaginatedAsync(parameters, cancellationToken);

        var invoices = paginatedInvoices
            .Items.Where(i => i.ClientId == request.ClientId.ToObjectId() && !i.IsDeleted)
            .ToList();

        // Collect items from invoices
        var allItems = new List<Models.Models.Item>();

        // Add items from invoices
        // foreach (var invoice in invoices)
        // {
        //     if (invoice.ItemsId != null && invoice.ItemsId.Any())
        //         allItems.AddRange(invoice.ItemsId);
        // }

        // Add items from client
        if (client.Items != null && client.Items.Any())
            allItems.AddRange(client.Items);

        // Map to DTOs
        var dtos = allItems.Select(i => mapper.Map<ItemResponse>(i)).ToList();

        return new SuccessResponse<List<ItemResponse>>(dtos, "Items retrieved successfully");
    }
}
