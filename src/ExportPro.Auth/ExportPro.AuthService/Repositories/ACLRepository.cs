using ExportPro.Auth.SDK.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.AuthService.Repositories;

public class ACLRepository(ICollectionProvider collectionProvider)
    : BaseRepository<UserClientRoles>(collectionProvider),
        IACLRepository
{
    public Task<List<UserClientRoles>> GetUserClientRolesAsync(
        ObjectId userId,
        ObjectId clientId,
        CancellationToken cancellationToken = default
    )
    {
        return Collection.Find(x => x.UserId == userId && x.ClientId == clientId).ToListAsync(cancellationToken);
    }

    public Task<List<UserClientRoles>> GetUserRolesAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        return Collection.Find(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public Task RemoveUserClientRoleAsync(
        ObjectId userId,
        ObjectId clientId,
        CancellationToken cancellationToken = default
    )
    {
        return Collection.DeleteManyAsync(x => x.UserId == userId && x.ClientId == clientId, cancellationToken);
    }

    public Task DeleteRolesAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        return Collection.DeleteManyAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<List<UserClientRoles>> GetClientIdsForUserAsync(
        ObjectId userId,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<UserClientRoles>.Filter.Eq(x => x.UserId, userId);

        return await Collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateUserClientRoleAsync(
        ObjectId userId,
        ObjectId clientId,
        UserRole newRole,
        CancellationToken cancellationToken = default
    )
    {
        var filter = Builders<UserClientRoles>.Filter.And(
            Builders<UserClientRoles>.Filter.Eq(x => x.UserId, userId),
            Builders<UserClientRoles>.Filter.Eq(x => x.ClientId, clientId)
        );

        var update = Builders<UserClientRoles>.Update.Set(x => x.Role, newRole);

        var updateRes = await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return updateRes.MatchedCount > 0;
    }
}
