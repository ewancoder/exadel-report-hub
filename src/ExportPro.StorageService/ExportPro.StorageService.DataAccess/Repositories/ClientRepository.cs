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

public class ClientRepository(ICollectionProvider collectionProvider, IMapper mapper)
    : BaseRepository<Client>(collectionProvider),
        IClientRepository
{
    public Task<List<Client>> GetClients(int top, int skip, CancellationToken cancellationToken = default)
    {
        var clients = Collection.Find(_ => !_.IsDeleted);
        string message = "Clients Retrieved";
        var paginated = clients.Skip(skip).Limit(top).ToListAsync(cancellationToken);
        return paginated;
    }

    public async Task<ClientResponse> UpdateClient(
        ClientDto client,
        string clientid,
        CancellationToken cancellationToken
    )
    {
        var clientGet = await GetByIdAsync(ObjectId.Parse(clientid), cancellationToken);
        clientGet.Name = client.Name;
        clientGet.Description = client.Description;
        await UpdateOneAsync(clientGet, CancellationToken.None);
        return mapper.Map<ClientResponse>(clientGet);
    }

    public Task<bool> HigherThanMaxSize(int skip, CancellationToken cancellationToken = default)
    {
        var max_size = Collection.Find(_ => !_.IsDeleted).CountDocuments();
        if (skip > max_size)
            return Task.FromResult(true);
        return Task.FromResult(false);
    }

    public async Task AddItem(ObjectId id, Client updatedClient, CancellationToken cancellationToken = default)
    {
        var result = await Collection.ReplaceOneAsync(
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

        var result = await Collection.UpdateOneAsync(
            x => x.Id == clientId,
            update,
            cancellationToken: cancellationToken
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveItemFromClient(
        ObjectId clientId,
        ObjectId itemId,
        CancellationToken cancellationToken = default
    )
    {
        var update = Builders<Client>.Update.PullFilter(c => c.Items, i => i.Id == itemId);

        var result = await Collection.UpdateOneAsync(
            c => c.Id == clientId,
            update,
            cancellationToken: cancellationToken
        );

        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateItemInClient(
        ObjectId clientId,
        Item updatedItem,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<Client>.Filter.And(
            Builders<Client>.Filter.Eq(c => c.Id, clientId),
            Builders<Client>.Filter.ElemMatch(c => c.Items, i => i.Id == updatedItem.Id)
        );

        var update = Builders<Client>
            .Update.Set(c => c.Items[-1].Name, updatedItem.Name)
            .Set(c => c.Items[-1].Description, updatedItem.Description)
            .Set(c => c.Items[-1].Price, updatedItem.Price)
            .Set(c => c.Items[-1].CurrencyId, updatedItem.CurrencyId)
            .Set(c => c.Items[-1].Status, updatedItem.Status)
            .Set(c => c.Items[-1].UpdatedAt, DateTime.UtcNow);

        var result = await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<int> UpdateItemsInClient(
        ObjectId clientId,
        List<Item> updatedItems,
        CancellationToken cancellationToken = default
    )
    {
        int successCount = 0;
        foreach (var item in updatedItems)
        {
            var filter = Builders<Client>.Filter.And(
                Builders<Client>.Filter.Eq(c => c.Id, clientId),
                Builders<Client>.Filter.ElemMatch(c => c.Items, i => i.Id == item.Id)
            );

            var update = Builders<Client>
                .Update.Set(c => c.Items[-1].Name, item.Name)
                .Set(c => c.Items[-1].Description, item.Description)
                .Set(c => c.Items[-1].Price, item.Price)
                .Set(c => c.Items[-1].CurrencyId, item.CurrencyId)
                .Set(c => c.Items[-1].Status, item.Status)
                .Set(c => c.Items[-1].UpdatedAt, DateTime.UtcNow);

            var result = await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            if (result.ModifiedCount > 0)
                successCount++;
        }
        return successCount;
    }

    public async Task<Plans> AddPlanToClient(
        string clientId,
        PlansDto plan,
        CancellationToken cancellationToken = default
    )
    {
        var client = await GetByIdAsync(ObjectId.Parse(clientId), cancellationToken);
        var plans = mapper.Map<Plans>(plan);
        plans.Id = ObjectId.GenerateNewId();
        int ind = 0;
        foreach (var i in plans.items)
        {
            ind++;
            i.Id = ObjectId.GenerateNewId();
        }
        client.Plans.Add(plans);
        plans.Amount = ind;
        UpdateOneAsync(client, cancellationToken);
        return plans;
    }

    public async Task<PlansResponse> RemovePlanFromClient(
        string clientId,
        string planId,
        CancellationToken cancellationToken = default
    )
    {
        var client = await GetByIdAsync(ObjectId.Parse(clientId), cancellationToken);
        var plan = new Plans();
        foreach (var i in client.Plans)
        {
            if (i.Id.ToString() == planId)
            {
                plan = i;
                i.IsDeleted = true;
                break;
            }
        }
        await UpdateOneAsync(client, cancellationToken);
        var planResp = mapper.Map<PlansResponse>(plan);
        return planResp;
    }

    public Task<PlansResponse> UpdateClientPlan(
        string clientId,
        string planId,
        PlansDto plansDto,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }

    public Task<PlansResponse> UpdateClientPlan(string clientId, string planId, PlansDto plansDto)
    {
        throw new NotImplementedException();
    }

    //public async Task<PlansResponse> UpdateClientPlan(string clientId, string planId, PlansDto plansDto)
    //{
    //    var client = await GetClientById(clientId);
    //    Plans plans = new();
    //    foreach (var i in client.Plans)
    //    {
    //        if (i.Id.ToString() == planId)
    //        {
    //            plans = i;
    //        }
    //    }
    //    plans.StartDate = plansDto.StartDate;
    //    plans.EndDate = plansDto.EndDate;
    //    for (int i = 0; i < plansDto.Items.Count; ++i)
    //    {
    //        plans.items[i].Name = plansDto.Items[i].Name;
    //        plans.items[i].Description = plansDto.Items[i].Description;
    //        plans.items[i].Price = plansDto.Items[i].Price;
    //        plans.items[i].Currency = plansDto.Items[i].Currency;
    //        plans.items[i].Status = plansDto.Items[i].Status;
    //    }
    //    await UpdateOneAsync(client, CancellationToken.None);
    //    var planResp = mapper.Map<PlansResponse>(plans);
    //    return planResp;
    //}
}
