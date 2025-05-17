using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.PaginationParams;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<PaginatedList<ClientResponse>> GetClients(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );
    Task<bool> HigherThanMaxSize(int skip, CancellationToken cancellationToken = default);
    Task AddItem(ObjectId id, Client updatedClient, CancellationToken cancellationToken = default);
    Task<bool> AddItems(ObjectId clientId, List<Item> items, CancellationToken cancellationToken = default);
    Task<bool> RemoveItemFromClient(ObjectId clientId, ObjectId itemId, CancellationToken cancellationToken = default);
    Task<bool> UpdateItemInClient(ObjectId clientId, Item updatedItem, CancellationToken cancellationToken = default);

    Task<int> UpdateItemsInClient(
        ObjectId clientId,
        List<Item> updatedItems,
        CancellationToken cancellationToken = default
    );

    Task<Plans> AddPlanToClient(ObjectId clientId, PlansDto plan, CancellationToken cancellationToken = default);
    Task<PlansResponse> RemovePlanFromClient(ObjectId planId, CancellationToken cancellationToken = default);

    Task<PlansResponse?> UpdateClientPlan(
        ObjectId planId,
        PlansDto plansDto,
        CancellationToken cancellationToken = default
    );

    Task<PlansResponse?> GetPlan(ObjectId planId, CancellationToken cancellationToken = default);

    Task<PaginatedList<PlansResponse>> GetClientPlans(
        ObjectId clientId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );
}
