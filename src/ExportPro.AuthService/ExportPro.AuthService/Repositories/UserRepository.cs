using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.Common.Shared.Models;
using MongoDB.Driver;

namespace ExportPro.AuthService.Repositories;

public class UserRepository(ExportProMongoContext context) : IUserRepository
{
    private readonly IMongoCollection<User> _users = context.Database.GetCollection<User>("Users");

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve.</param>
    /// <returns>The user, or null if not found.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a user by their refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to find the user.</param>
    /// <returns>The user associated with the refresh token, or null if not found.</returns>
    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _users
            .Find(u => u.RefreshTokens
                .Any(rt => rt.Token == refreshToken))
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>The created user.</returns>
    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
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
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
