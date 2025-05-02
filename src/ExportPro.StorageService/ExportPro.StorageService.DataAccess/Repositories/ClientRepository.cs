using System.Security.Claims;
using AutoMapper;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using ExportPro.StorageService.Models.Models;
using ExportPro.StorageService.SDK.DTOs;
using ExportPro.StorageService.SDK.Responses;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories;

public sealed class ClientRepository(
    IHttpContextAccessor httpContextAccessor,
    ICollectionProvider collectionProvider,
    IMapper mapper
) : BaseRepository<Client>(collectionProvider), IClientRepository
{
    public Task<List<Client>> GetClients(int top, int skip, CancellationToken cancellationToken = default)
    {
        var clients = Collection.Find(c => !c.IsDeleted);
        var paginated = clients.Skip(skip).Limit(top).ToListAsync(cancellationToken);
        return paginated;
    }

    public Task<bool> HigherThanMaxSize(int skip, CancellationToken cancellationToken = default)
    {
        var maxSize = Collection.Find(c => !c.IsDeleted).CountDocuments(cancellationToken);
        if (skip > maxSize)
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
            throw new InvalidOperationException($"No client found with ID {id} to replace.");
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
            .Update.Set(c => c.Items![-1].Name, updatedItem.Name)
            .Set(c => c.Items![-1].Description, updatedItem.Description)
            .Set(c => c.Items![-1].Price, updatedItem.Price)
            .Set(c => c.Items![-1].CurrencyId, updatedItem.CurrencyId)
            .Set(c => c.Items![-1].Status, updatedItem.Status)
            .Set(c => c.Items![-1].UpdatedAt, DateTime.UtcNow);

        var result = await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<int> UpdateItemsInClient(
        ObjectId clientId,
        List<Item> updatedItems,
        CancellationToken cancellationToken = default
    )
    {
        var successCount = 0;
        foreach (var item in updatedItems)
        {
            var filter = Builders<Client>.Filter.And(
                Builders<Client>.Filter.Eq(c => c.Id, clientId),
                Builders<Client>.Filter.ElemMatch(c => c.Items, i => i.Id == item.Id)
            );

            var update = Builders<Client>
                .Update.Set(c => c.Items![-1].Name, item.Name)
                .Set(c => c.Items![-1].Description, item.Description)
                .Set(c => c.Items![-1].Price, item.Price)
                .Set(c => c.Items![-1].CurrencyId, item.CurrencyId)
                .Set(c => c.Items![-1].Status, item.Status)
                .Set(c => c.Items![-1].UpdatedAt, DateTime.UtcNow);

            var result = await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            if (result.ModifiedCount > 0)
                successCount++;
        }

        return successCount;
    }

    public async Task<Plans> AddPlanToClient(
        ObjectId clientId,
        PlansDto plan,
        CancellationToken cancellationToken = default
    )
    {
        var client = await GetOneAsync(x => x.Id == clientId && !x.IsDeleted, cancellationToken);
        var plans = mapper.Map<Plans>(plan);
        plans.Id = ObjectId.GenerateNewId();
        var ind = 0;
        foreach (var i in plans.items)
        {
            ind++;
            i.Id = ObjectId.GenerateNewId();
        }

        client!.Plans?.Add(plans);
        plans.Amount = ind;
        client.UpdatedAt = DateTime.UtcNow;
        client.UpdatedBy = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        await UpdateOneAsync(client, cancellationToken);
        return plans;
    }

    public async Task<PlansResponse> RemovePlanFromClient(
        ObjectId planId,
        CancellationToken cancellationToken = default
    )
    {
        var client = await Collection
            .Find(x => x.Plans!.Any(y => y.Id == planId) && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        var plan = client.Plans!.FirstOrDefault(x => x.Id == planId);
        client.Plans?.Remove(plan!);
        await UpdateOneAsync(client, cancellationToken);
        var planResp = mapper.Map<PlansResponse>(plan);
        return planResp;
    }

    public async Task<PlansResponse?> UpdateClientPlan(
        ObjectId planId,
        PlansDto plansDto,
        CancellationToken cancellationToken = default
    )
    {
        var objectId = planId;
        var client = await Collection
            .Find(x => x.Plans!.Any(p => p.Id == objectId) && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        var plan = client.Plans!.FirstOrDefault(p => p.Id == objectId);
        if (plan == null)
            return null;
        if (plansDto.StartDate != DateTime.MinValue)
            plan.StartDate = plansDto.StartDate;
        if (plansDto.EndDate != DateTime.MinValue)
            plan.EndDate = plansDto.EndDate;

        var items = plansDto.Items.Select(x => mapper.Map<Item>(x)).ToList();
        foreach (var item in items)
            item.Id = ObjectId.GenerateNewId();
        plan.items = items;

        plan.UpdatedBy = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        plan.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<Client>.Filter.And(
            Builders<Client>.Filter.Eq(c => c.Id, client.Id),
            Builders<Client>.Filter.ElemMatch(c => c.Plans, p => p.Id == objectId)
        );
        var update = Builders<Client>.Update.Set("Plans.$", plan);
        await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        var planResponse = mapper.Map<PlansResponse>(plan);
        return planResponse;
    }

    public async Task<PlansResponse?> GetPlan(ObjectId planId, CancellationToken cancellationToken = default)
    {
        var client = await Collection
            .Find(x => x.Plans!.Any(y => y.Id == planId) && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
        if (client == null)
            return null!;
        var plan = client.Plans?.FirstOrDefault(x => x.Id == planId && !x.IsDeleted);
        var planResponse = mapper.Map<PlansResponse>(plan);
        return planResponse;
    }

    public async Task<List<PlansResponse>> GetClientPlans(
        ObjectId clientId,
        int top,
        int skip,
        CancellationToken cancellationToken = default
    )
    {
        var client = await GetOneAsync(x => x.Id == clientId && !x.IsDeleted, cancellationToken);
        var plans = client!.Plans!.Skip(skip).Take(top).Select(x => mapper.Map<PlansResponse>(x)).ToList();
        return plans;
    }
}
