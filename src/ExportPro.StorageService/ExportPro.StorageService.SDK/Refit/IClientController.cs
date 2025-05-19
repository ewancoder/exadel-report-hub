using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IClientController
{
    [Post("/api/client")]
    Task<BaseResponse<ClientResponse>> CreateClient(
        [Body] ClientDto client,
        CancellationToken cancellationToken = default
    );

    [Get("/api/client")]
    Task<BaseResponse<PaginatedList<ClientResponse>>> GetClients(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );

    [Get("/api/client/{clientId}")]
    Task<BaseResponse<ClientResponse>> GetClientById(Guid clientId, CancellationToken cancellationToken = default);

    [Patch("/api/client/{clientId}")]
    Task<BaseResponse<ClientResponse>> UpdateClient(
        Guid clientId,
        [Body] ClientDto client,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/client/{clientId}")]
    Task<BaseResponse<ClientResponse>> SoftDeleteClient(Guid clientId, CancellationToken cancellationToken = default);

    [Get("/api/client/{clientId}/items")]
    Task<BaseResponse<PaginatedList<ItemResponse>>> GetClientItems(
        Guid clientId,
        [Query] PaginationParameters parameters,
        CancellationToken cancellationToken = default
    );

    [Patch("/api/client/{clientId}/item")]
    Task<BaseResponse<Guid>> AddItemToClient(
        Guid clientId,
        [Body] ItemDtoForClient item,
        CancellationToken cancellationToken = default
    );

    [Patch("/api/client/{clientId}/items")]
    Task<BaseResponse<bool>> AddItemsToClient(
        Guid clientId,
        [Body] List<ItemDtoForClient> items,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/client/{clientId}/item/{itemId}")]
    Task<BaseResponse<bool>> RemoveItemFromClient(
        Guid clientId,
        Guid itemId,
        CancellationToken cancellationToken = default
    );

    [Put("/api/client/{clientId}/item")]
    Task<BaseResponse<bool>> UpdateItemInClient(
        Guid clientId,
        [Body] Item item,
        CancellationToken cancellationToken = default
    );

    [Put("/api/client/{clientId}/items")]
    Task<BaseResponse<bool>> UpdateItemsInClient(
        Guid clientId,
        [Body] List<Item> items,
        CancellationToken cancellationToken = default
    );

    [Get("/api/client/plan/{planId}")]
    Task<BaseResponse<PlansResponse>> GetPlan(Guid planId, CancellationToken cancellationToken = default);

    [Get("/api/client/{clientId}/plans")]
    Task<BaseResponse<PaginatedList<PlansResponse>>> GetClientPlans(
        Guid clientId,
        [Query] PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );

    [Patch("/api/client/{clientId}/plan")]
    Task<BaseResponse<PlansResponse>> AddPlanToClient(
        Guid clientId,
        [Body] PlansDto plan,
        CancellationToken cancellationToken = default
    );

    [Delete("/api/client/plan/{planId}")]
    Task<BaseResponse<PlansResponse>> RemovePlanFromClient(Guid planId, CancellationToken cancellationToken = default);

    [Patch("/api/client/plan/{planId}")]
    Task<BaseResponse<PlansResponse>> UpdateClientPlan(
        Guid planId,
        [Body] PlansDto plansDto,
        CancellationToken cancellationToken = default
    );
}
