using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    BaseResponse<Task<List<Client>>> GetClients(int top, int skip);
    Task<Client> GetClientById(string Clientid);
    Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto);
    Task<ClientResponse> UpdateClient(ClientUpdateDto client, string clientid);
    Task SoftDeleteClient(string clientId);
    Task<bool> ClientExists(string Name);
    Task<bool> HigherThanMaxSize(int skip);
    Task AddItem(ObjectId id, Client updatedClient, CancellationToken cancellationToken = default);
    Task<bool> AddItems(ObjectId clientId, List<Item> items, CancellationToken cancellationToken = default);
    Task<bool> RemoveItemFromClient(ObjectId clientId, ObjectId itemId, CancellationToken cancellationToken = default);
    Task<bool> UpdateItemInClient(ObjectId clientId, Item updatedItem, CancellationToken cancellationToken = default);
    Task<int> UpdateItemsInClient(
        ObjectId clientId,
        List<Item> updatedItems,
        CancellationToken cancellationToken = default
    );
    Task<Plans> AddPlanToClient(string clientId, PlansDto plan, CancellationToken cancellationToken = default);
    Task<PlansResponse> RemovePlanFromClient(
        string clientId,
        string planId,
        CancellationToken cancellationToken = default
    );
}
