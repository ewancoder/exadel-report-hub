using System.Net;
using AutoMapper;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.CQRS.QueryHandlers.ItemQueries;

public record GetItemsQuery(string ClientId) : IQuery<List<ItemResponse>>;
public record GetItemByIdQuery(string ClientId, string ItemId) : IQuery<ItemResponse>;

public class GetItemsQueryHandler(
    IClientRepository repository,
    IMapper mapper
) : IQueryHandler<GetItemsQuery, List<ItemResponse>>
{
    private readonly IClientRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<List<ItemResponse>>> Handle(
        GetItemsQuery request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.ClientId, out var clientObjectId))
        {
            return new BaseResponse<List<ItemResponse>>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Invalid client ID format."]
            };
        }

        var client = await _repository.GetOneAsync(
            c => c.Id == clientObjectId && !c.IsDeleted,
            cancellationToken
        );
        if (client == null)
        {
            return new BaseResponse<List<ItemResponse>>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Client not found."]
            };
        }

        var items = client.Items ?? new List<Item>();
        var dtos = items.Select(i => _mapper.Map<ItemResponse>(i)).ToList();

        return new SuccessResponse<List<ItemResponse>>(dtos, "Items retrieved successfully");
    }
}

public class GetItemByIdQueryHandler(
    IClientRepository repository,
    IMapper mapper
) : IQueryHandler<GetItemByIdQuery, ItemResponse>
{
    private readonly IClientRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    public async Task<BaseResponse<ItemResponse>> Handle(
        GetItemByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(request.ClientId, out var clientObjectId) ||
            !ObjectId.TryParse(request.ItemId, out var itemObjectId))
        {
            return new BaseResponse<ItemResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.BadRequest,
                Messages = ["Invalid client or item ID format."]
            };
        }

        var client = await _repository.GetOneAsync(
            c => c.Id == clientObjectId && !c.IsDeleted,
            cancellationToken
        );
        if (client == null)
        {
            return new BaseResponse<ItemResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Client not found."]
            };
        }

        var item = client.Items?.FirstOrDefault(i => i.Id == itemObjectId && !i.IsDeleted);
        if (item == null)
        {
            return new BaseResponse<ItemResponse>
            {
                IsSuccess = false,
                ApiState = HttpStatusCode.NotFound,
                Messages = ["Item not found."]
            };
        }

        var dto = _mapper.Map<ItemResponse>(item);
        return new SuccessResponse<ItemResponse>(dto, "Item retrieved successfully");
    }
}
