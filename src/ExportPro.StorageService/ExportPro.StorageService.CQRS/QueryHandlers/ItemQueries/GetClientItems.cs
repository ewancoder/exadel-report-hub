using AutoMapper;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ItemQueries;

public record GetClientItemsQuery(Guid ClientId, PaginationParameters Parameters) : IQuery<PaginatedList<ItemResponse>>;

public class GetClientItemsQueryHandler(
    IClientRepository clientRepository,
    ICurrencyRepository currencyRepository,
    IMapper mapper
) : IQueryHandler<GetClientItemsQuery, PaginatedList<ItemResponse>>
{
    public async Task<BaseResponse<PaginatedList<ItemResponse>>> Handle(
        GetClientItemsQuery request,
        CancellationToken cancellationToken
    )
    {
        var items = await clientRepository.GetOneAsync(
            x => x.Id == request.ClientId.ToObjectId() && !x.IsDeleted,
            cancellationToken
        );
        if (items == null)
            return new BadRequestResponse<PaginatedList<ItemResponse>>("Client not found.");

        var clientItems = items.Items!;
        List<ItemResponse> itemsDto = new();
        for (int i = 0; i < clientItems.Count; i++)
        {
            var currency = await currencyRepository.GetOneAsync(
                x => x.Id == clientItems[i].CurrencyId && !x.IsDeleted,
                cancellationToken
            );
            ItemResponse dto = mapper.Map<ItemResponse>(clientItems[i]);
            dto.Currency = currency?.CurrencyCode;
            itemsDto.Add(dto);
        }
        return new SuccessResponse<PaginatedList<ItemResponse>>(
            itemsDto.ToPaginatedList(pageSize: request.Parameters.PageSize, pageNumber: request.Parameters.PageNumber),
            "Client items retrieved successfully"
        );
    }
}
