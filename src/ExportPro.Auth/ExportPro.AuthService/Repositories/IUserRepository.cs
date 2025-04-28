using ExportPro.Auth.SDK.Models;
using MongoDB.Bson;

namespace ExportPro.AuthService.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<User?> GetByEmailAsync(string email);

}
