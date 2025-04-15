using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using AutoMapper;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.Common.Shared.Library;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace ExportPro.StorageService.DataAccess.Repositories;

public class ClientRepository : MongoRepositoryBase<Client>, IClientRepository
{
    private IMongoCollection<Client> _clients;
    private readonly IMongoDbConnectionFactory _mongoDbConnectionFactory;
    private readonly IMapper _mapper;

    public ClientRepository(
        IMapper mapper,
        IMongoDbConnectionFactory mongoDbConnectionFactory,
        ICollectionProvider collectionProvider
    )
        : base(collectionProvider)
    {
        _mapper = mapper;
        _clients = collectionProvider.GetCollection<Client>("Client");
    }

    //public (string Messege, Task<List<Client>>) GetClients(int client_size=5,int page=1)
    //{
    //    var clients = _clients.Find(_ => true);
    //    string Messege = "Clients Retrieved";
    //    var size = clients.CountDocuments();
    //    int max_page = (int)Math.Ceiling(size / (double)client_size);
    //    if (max_page<page) Messege= $"Page Size Exceeded {max_page}.Retriving Last Page";
    //    var paginated= clients.Skip((page-1) * client_size).Limit(client_size).ToListAsync();
    //    return (Messege,paginated);
    //}

    public Task<Client> GetClientByName(string name)
    {
        return _clients.Find(x => x.Name == name).FirstOrDefaultAsync();
    }

    public BaseResponse<Task<List<Client>>> GetClients(int top, int skip)
    {
        var clients = _clients.Find(_ => true);
        string message = "Clients Retrieved";
        var size = clients.CountDocuments();
        if (size == 0)
        {
            message = $"There is no such document";
            return new BaseResponse<Task<List<Client>>> { Messages = [message], Data = null };
        }
        if (skip > size)
        {
            message = $"Skip Exceeded the size {size}";
            skip = (int)size;
        }
        var paginated = clients.Skip(skip).Limit(top).ToListAsync();
        return new BaseResponse<Task<List<Client>>>
        {
            Messages = [message],
            Data = paginated,
            ApiState = HttpStatusCode.Accepted,
            IsSuccess = true,
        };
    }

    public Task<Client> GetClientById(string Clientid)
    {
        var client = GetOneAsync(x => x.Id == ObjectId.Parse(Clientid), CancellationToken.None);
        return client;
    }

    public Task<List<ClientResponse>> GetAllCLientsIncludingSoftDeleted()
    {
        throw new NotImplementedException();
    }

    public async Task<ClientResponse> AddClientFromClientDto(ClientDto clientDto)
    {
        Client client = _mapper.Map<Client>(clientDto);
        foreach (var item in client.Items)
        {
            item.Id = ObjectId.GenerateNewId();
        }
        client.CreatedAt = DateTime.UtcNow;
        client.UpdatedAt = null;
        await AddOneAsync(client, CancellationToken.None);
        var clientresp = _mapper.Map<ClientResponse>(client);
        clientresp.itemResponses = _mapper.Map<List<ItemResponse>>(client.Items);
        return clientresp;
    }

    public Task<ClientResponse> UpdateClient(ClientUpdateDto client, string clientid)
    {
        var item = new Item();
        throw new NotImplementedException();
    }

    public Task<string> SoftDeleteClient(string client_id)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteClient(ObjectId Clientid)
    {
        throw new NotImplementedException();
    }

    public Task<ClientResponse> GetClientByIdIncludingSoftDeleted(ObjectId ClientId)
    {
        throw new NotImplementedException();
    }

    public Task<ClientResponse> AddItemIds(string Clientid, List<string> ItemIds)
    {
        throw new NotImplementedException();
    }

    public Task<ClientResponse> AddInvoiceIds(string Clientid, List<string> InvoiceIds)
    {
        throw new NotImplementedException();
    }

    public Task<ClientResponse> AddCustomerIds(string Clientid, List<string> customerids)
    {
        throw new NotImplementedException();
    }

    public Task<FullClientResponse> GetFullClient(string clientid)
    {
        throw new NotImplementedException();
    }

    public Task<List<FullClientResponse>> GetAllFullClients()
    {
        throw new NotImplementedException();
    }

    public Task<string> SoftDeleteClient(ObjectId Clientid)
    {
        throw new NotImplementedException();
    }
}
