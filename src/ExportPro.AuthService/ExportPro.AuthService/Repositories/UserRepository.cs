using ExportPro.Common.DataAccess.MongoDB.Contexts;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Models;
using MongoDB.Driver;

namespace ExportPro.AuthService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(ExportProMongoContext context)
    {
        _users = context.Database.GetCollection<User>("Users");
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        // Look for any user with a matching refresh token in their token list
        return await _users
            .Find(u => u.RefreshTokens
                .Any(rt => rt.Token == refreshToken))
            .FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
