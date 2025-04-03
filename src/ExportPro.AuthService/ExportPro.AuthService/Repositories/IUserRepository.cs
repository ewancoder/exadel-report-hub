using ExportPro.Common.Shared.Models;

namespace ExportPro.AuthService.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
}
