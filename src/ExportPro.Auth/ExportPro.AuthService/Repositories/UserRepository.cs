using ExportPro.Auth.SDK.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.DataAccess.MongoDB.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExportPro.AuthService.Repositories;

public class UserRepository : MongoRepositoryBase<User>, IUserRepository
{
    private readonly ICollectionProvider collection;

    public UserRepository(ICollectionProvider collectionProvider) : base(collectionProvider)
    {
        collection = collectionProvider;
        EnsureEmailUniqueness();
    }

    private IMongoCollection<User> Collection => collection.GetCollection<User>();

    //private readonly IMongoCollection<User> _users;

    //public UserRepository(ExportProMongoContext context, IMongoDbConnectionFactory mongoDbConnectionFactory)
    //{
    //    _users = mongoDbConnectionFactory.GetDatabase().GetCollection<User>("Users");
    //    EnsureEmailUniqueness();
    //}

    /// <summary>
    /// Ensures that the email field in the User collection is unique by creating a unique index on it.
    /// </summary>
    private void EnsureEmailUniqueness()
    {
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<User>(indexKeys, indexOptions);
        Collection.Indexes.CreateOne(indexModel);
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user, or null if not found.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await Collection.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a user by their email.
    /// </summary>
    /// <param name="email">The email of the user to retrieve.</param>
    /// <returns>The user, or null if not found.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Collection.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a user by their refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to find the user.</param>
    /// <returns>The user associated with the refresh token, or null if not found.</returns>
    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await Collection
            .Find(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken))
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    /// <param name="id">The ObjectId of the user to retrieve.</param>
    /// <returns>The user, or null if not found.</returns>
    public async Task<User?> GetByIdAsync(ObjectId id)
    {
        return await Collection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>The created user.</returns>
    public async Task<User> CreateAsync(User user)
    {
        await Collection.InsertOneAsync(user);
        return user;
    }

    /// <summary>
    /// Updates a user in the database.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <remarks>
    /// This method completely replaces the existing user document in the database with the given user.
    /// </remarks>
    public async Task UpdateAsync(User user)
    {
        await Collection.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
