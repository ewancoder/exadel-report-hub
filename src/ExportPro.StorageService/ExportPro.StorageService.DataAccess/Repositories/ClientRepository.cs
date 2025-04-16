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
        var max_size = clients.CountDocuments();
        if (max_size == 0)
        {
            message = $"There is no such document";
            return new BaseResponse<Task<List<Client>>> { Messages = [message], Data = null };
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

    public async Task<bool> ClientExists(string Name)
    {
        var client = await GetOneAsync(x => x.Name == Name, CancellationToken.None);
        if (client == null)
            return false;
        return true;
    }

    public Task<bool> HigherThanMaxSize(int skip)
    {
        var max_size = _clients.Find(_ => true).CountDocuments();
        if (skip > max_size)
            return Task.FromResult(true);
        return Task.FromResult(false);
    }
    public async Task AddItem(ObjectId id, Client updatedClient, CancellationToken cancellationToken = default)
    {
        var result = await _clients.ReplaceOneAsync(
            client => client.Id == id,
            updatedClient,
            cancellationToken: cancellationToken
        );

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"No client found with ID {id} to replace.");
        }
    }

    public async Task<bool> AddItems(ObjectId clientId, List<Item> items, CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            item.Id = ObjectId.GenerateNewId();
            item.CreatedAt = DateTime.UtcNow;
        }

        var update = Builders<Client>.Update.PushEach(x => x.Items, items);

        var result = await _clients.UpdateOneAsync(
            x => x.Id == clientId,
            update,
            cancellationToken: cancellationToken
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveItemFromClient(ObjectId clientId, ObjectId itemId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Client>.Update.PullFilter(c => c.Items, i => i.Id == itemId);

        var result = await _clients.UpdateOneAsync(
            c => c.Id == clientId,
            update,
            cancellationToken: cancellationToken
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateItemInClient(ObjectId clientId, Item updatedItem, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Client>.Filter.And(
            Builders<Client>.Filter.Eq(c => c.Id, clientId),
            Builders<Client>.Filter.ElemMatch(c => c.Items, i => i.Id == updatedItem.Id)
        );

        var update = Builders<Client>.Update
            .Set(c => c.Items[-1].Name, updatedItem.Name)
            .Set(c => c.Items[-1].Description, updatedItem.Description)
            .Set(c => c.Items[-1].Price, updatedItem.Price)
            .Set(c => c.Items[-1].Currency, updatedItem.Currency)
            .Set(c => c.Items[-1].Status, updatedItem.Status)
            .Set(c => c.Items[-1].UpdatedAt, DateTime.UtcNow);

        var result = await _clients.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<int> UpdateItemsInClient(ObjectId clientId, List<Item> updatedItems, CancellationToken cancellationToken = default)
    {
        int successCount = 0;
        foreach (var item in updatedItems)
        {
            var filter = Builders<Client>.Filter.And(
                Builders<Client>.Filter.Eq(c => c.Id, clientId),
                Builders<Client>.Filter.ElemMatch(c => c.Items, i => i.Id == item.Id)
            );

            var update = Builders<Client>.Update
                .Set(c => c.Items[-1].Name, item.Name)
                .Set(c => c.Items[-1].Description, item.Description)
                .Set(c => c.Items[-1].Price, item.Price)
                .Set(c => c.Items[-1].Currency, item.Currency)
                .Set(c => c.Items[-1].Status, item.Status)
                .Set(c => c.Items[-1].UpdatedAt, DateTime.UtcNow);

            var result = await _clients.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            if (result.ModifiedCount > 0) successCount++;
        }

        return successCount;
    }
}
