using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;

namespace ExportPro.StorageService.DataAccess.Services;

public interface IClientService
{
    Task<List<ClientResponse>> GetClientsService();
    Task<ClientResponse> GetClientById(string Clientid);
    Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted();
    Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto);
    Task<ClientResponse> UpdateClient(ClientUpdateDto client,string clientid);
    Task<string> SoftDeleteClient(ObjectId Clientid);
    Task<string> DeleteClient(ObjectId Clientid);
    Task<ClientResponse> GetClientByIdIncludingSoftDeleted(ObjectId ClientId);
}