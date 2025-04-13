using ExportPro.StorageService.DataAccess.Repositories;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Mapping;
using ExportPro.StorageService.SDK.Responses;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Readers.Rar;

namespace ExportPro.StorageService.DataAccess.Services;

public class ClientService:IClientService
{
    private readonly ClientRepository _clientRepository;
    public ClientService(ClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<List<ClientResponse>> GetClientsService()
    {
        var clients = await _clientRepository.GetClients();
        var clientresponse = clients.Where(x=>x.IsDeleted==false).Select(x => ClientToClientResponse.ClientToClientReponse(x)).ToList();
        return clientresponse;
    }
    public async Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto)
    {
        Client client = new()
        {
            Name = clientDto.Name,
            Description = clientDto.Description
        };
        await _clientRepository.AddOneAsync(client, CancellationToken.None);
        return ClientToClientResponse.ClientToClientReponse(client);
    }

    public async Task<ClientResponse> GetClientByIdIncludingSoftDeleted(ObjectId ClientId)
    {
        var client =await _clientRepository.GetOneAsync(x=>x.Id==ClientId,CancellationToken.None);
        if (client == null) return null;
        var clientresponse= ClientToClientResponse.ClientToClientReponse(client) ;
        return clientresponse;
    }
    public async Task<ClientResponse> GetClientById(string Clientid)
    {
        var client =await _clientRepository.GetOneAsync(x=>x.Id.ToString()==Clientid && x.IsDeleted==false, CancellationToken.None);
        if (client == null) return null;
        var clientresponse= ClientToClientResponse.ClientToClientReponse(client) ;
        return clientresponse;
    }

    public async Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted()
    {
        var clients =await _clientRepository.GetClients();
        var clientresponses=clients.Select(x => ClientToClientResponse.ClientToClientReponse(x)).ToList();
        return clientresponses;
    }

    public async Task<ClientResponse> UpdateClient(ClientUpdateDto clientUpdateDto,string clientid)
    {
     
     Client client = await _clientRepository.GetOneAsync(x=>x.Id.ToString()==clientid, CancellationToken.None);
        if (clientUpdateDto.Name != null)  client.Name=clientUpdateDto.Name;
        if (clientUpdateDto.Description != null) client.Description = clientUpdateDto.Description;
        client.IsDeleted =clientUpdateDto.IsDeleted;
        client.UpdatedAt=DateTime.UtcNow;
        await _clientRepository.UpdateOneAsync(client, CancellationToken.None);
        var after = ClientToClientResponse.ClientToClientReponse(client);
        return after;
    }

    public async Task<string> SoftDeleteClient(ObjectId Clientid)
    {
        await _clientRepository.SoftDeleteAsync(Clientid, CancellationToken.None);
        return "Soft Deleted";
    }

    public async Task<string> DeleteClient(ObjectId Clientid)
    {
        await _clientRepository.DeleteAsync(Clientid,CancellationToken.None);
        return "Deleted";
    }
}   