using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IStorageServiceApi
{
    [Post("/api/client/")]
    Task<BaseResponse<ClientResponse>> CreateClient([Body] ClientDto clientDto);

    [Get("/api/client/")]
    Task<BaseResponse<List<ClientResponse>>> GetClients([Query] int top = 5, [Query] int skip = 0);

    [Get("/api/client/{clientId}")]
    Task<BaseResponse<ClientResponse>> GetClientById(Guid clientId);

    [Patch("/api/client/{clientId}")]
    Task<BaseResponse<ClientResponse>> UpdateClient(Guid clientId, [Body] ClientDto client);

    [Delete("/api/client/{clientId}")]
    Task<BaseResponse<ClientResponse>> SoftDeleteClient(Guid clientId);

    [Patch("/api/client/{clientId}/item")]
    Task<BaseResponse<string>> AddItemToClient(Guid clientId, [Body] ItemDtoForClient item);

    [Patch("/api/client/{clientId}/items")]
    Task<BaseResponse<bool>> AddItemsToClient(Guid clientId, [Body] List<ItemDtoForClient> items);

    [Delete("/api/client/{clientId}/item/{itemId}")]
    Task<BaseResponse<bool>> RemoveItemFromClient(Guid clientId, Guid itemId);

    [Put("/api/client/{clientId}/item")]
    Task<BaseResponse<bool>> UpdateItemInClient(Guid clientId, [Body] Item item);

    [Put("/api/client/{clientId}/items")]
    Task<BaseResponse<bool>> UpdateItemsInClient(Guid clientId, [Body] List<Item> items);

    [Get("/api/client/plan/{planId}")]
    Task<BaseResponse<PlansResponse>> GetPlan(Guid planId);

    [Get("/api/client/{clientId}/plans")]
    Task<BaseResponse<List<PlansResponse>>> GetClientPlans(Guid clientId, [Query] int top = 5, [Query] int skip = 0);

    [Patch("/api/client/{clientId}/plan")]
    Task<BaseResponse<PlansResponse>> AddPlanToClient(Guid clientId, [Body] PlansDto plan);

    [Delete("/api/client/plan/{planId}")]
    Task<BaseResponse<PlansResponse>> RemovePlanFromClient(Guid planId);

    [Patch("/api/client/plan/{planId}")]
    Task<BaseResponse<PlansResponse>> UpdateClientPlan(Guid planId, [Body] PlansDto plansDto);
}
