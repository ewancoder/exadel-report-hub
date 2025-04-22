using ExportPro.Auth.SDK.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using ExportPro.StorageService.DataAccess.Interfaces;
using MongoDB.Driver;

namespace ExportPro.StorageService.DataAccess.Repositories
{
    public class UserRepository(ICollectionProvider collectionProvider) : MongoRepositoryBase<User>(collectionProvider), IUserRepository
    {
        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await Collection.Find(_ => true).ToListAsync(cancellationToken);
        }
    }
}
