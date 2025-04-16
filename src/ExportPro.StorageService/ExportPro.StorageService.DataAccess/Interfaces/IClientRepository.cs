using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Interfaces;

public interface IClientRepository : IRepository<Client>
{
    Task<Client> GetClientByName(string name);
    BaseResponse<Task<List<Client>>> GetClients(int top, int skip);
    Task<Client> GetClientById(string Clientid);
    Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted();
    Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto);
    Task<ClientResponse> UpdateClient(ClientUpdateDto client, string clientid);
    Task<string> SoftDeleteClient(ObjectId Clientid);
    Task<string> DeleteClient(ObjectId Clientid);
    Task<ClientResponse> GetClientByIdIncludingSoftDeleted(ObjectId ClientId);
    Task<ClientResponse> AddItemIds(string Clientid, List<string> ItemIds);
    Task<ClientResponse> AddInvoiceIds(string Clientid, List<string> InvoiceIds);
    Task<ClientResponse> AddCustomerIds(string Clientid, List<string> customerids);
    Task<FullClientResponse> GetFullClient(string clientid);
    Task<List<FullClientResponse>> GetAllFullClients();
    Task<bool> ClientExists(string Name);
    Task<bool> HigherThanMaxSize(int skip);
    Task AddItem(ObjectId id, Client updatedClient, CancellationToken cancellationToken = default);
    Task AddItems(ObjectId clientId, List<Item> items, CancellationToken cancellationToken = default);
}
