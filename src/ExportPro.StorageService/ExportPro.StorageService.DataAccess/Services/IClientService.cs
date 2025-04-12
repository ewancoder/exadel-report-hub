using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Services;

public interface IClientService
{
    Task<List<ClientResponse>> GetClientsService();
    Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto);
    Task<ClientResponse> GetClientById(string Clientid);
    Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted();
    Task<ClientResponse> UpdateClient(Client client);
    Task<string> SoftDeleteClient(ObjectId Clientid);
    Task<string> DeleteClient(ObjectId Clientid);
}