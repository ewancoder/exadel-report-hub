using ExportPro.Common.Shared.Models;

namespace ExportPro.Common.DataAccess.MongoDB.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
}
