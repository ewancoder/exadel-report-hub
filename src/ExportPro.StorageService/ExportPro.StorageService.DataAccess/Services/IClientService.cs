//using ExportPro.StorageService.Models.Models;
//using ExportPro.StorageService.SDK.DTOs;
//using ExportPro.StorageService.SDK.Responses;
//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Serializers;

//namespace ExportPro.StorageService.DataAccess.Services;

//public interface IclientRepository
//{
//    Task<List<ClientResponse>> GetClientsService(int client_size,int page);
//    Task<ClientResponse> GetClientById(string Clientid);
//    Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted();
//    Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto);
//    Task<ClientResponse> UpdateClient(ClientUpdateDto client,string clientid);
//    Task<string> SoftDeleteClient(ObjectId Clientid);
//    Task<string> DeleteClient(ObjectId Clientid);
//    Task<ClientResponse> GetClientByIdIncludingSoftDeleted(ObjectId ClientId);
//    Task<ClientResponse> AddItemIds(string Clientid, List<string> ItemIds);
//    Task<ClientResponse> AddInvoiceIds(string Clientid, List<string> InvoiceIds);
//    Task<ClientResponse> AddCustomerIds(string Clientid, List<string> customerids);
//    Task<FullClientResponse> GetFullClient(string clientid);
//    Task<List<FullClientResponse>> GetAllFullClients();
//}