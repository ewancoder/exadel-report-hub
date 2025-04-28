
using ExportPro.Auth.SDK.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.AuthService.Repositories;
public class ACLRepository(ICollectionProvider collectionProvider) : BaseRepository<UserClientRoles>(collectionProvider), IACLRepository
{
    public async Task<List<UserClientRoles>> GetUserClientRolesAsync(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default)
    {
        return await Collection
            .Find(x => x.UserId == userId && x.ClientId == clientId).ToListAsync(cancellationToken);
    }

    public async Task<List<UserClientRoles>> GetUserRolesAsync(ObjectId userId, CancellationToken cancellationToken = default)
    {
        return await Collection
            .Find(x => x.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task RemoveUserClientRoleAsync(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default)
    {
        var del = await Collection.DeleteManyAsync(
            x => x.UserId == userId && x.ClientId == clientId,
            cancellationToken
        );
        if(del.DeletedCount == 0)
        {
            throw new InvalidOperationException($"No user client role found with UserId {userId} and ClientId {clientId} to delete.");
        }
    }

    public async Task UpdateUserClientRoleAsync(ObjectId userId, ObjectId clientId, UserRole newRole, CancellationToken cancellationToken = default)
    {
        var filter = Builders<UserClientRoles>.Filter.And(
            Builders<UserClientRoles>.Filter.Eq(x => x.UserId, userId),
            Builders<UserClientRoles>.Filter.Eq(x => x.ClientId, clientId)
        );

        var update = Builders<UserClientRoles>.Update
            .Set(x => x.Role, newRole);

       var updateRes = await Collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (updateRes.MatchedCount == 0)
        {
            throw new InvalidOperationException($"No user client role found with UserId {userId} and ClientId {clientId} to update.");
        }
    }



}
