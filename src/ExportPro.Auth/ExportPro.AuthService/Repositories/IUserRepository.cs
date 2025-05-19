using ExportPro.Auth.SDK.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;

namespace ExportPro.AuthService.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetUsersAsync(CancellationToken cancellationToken);
}
