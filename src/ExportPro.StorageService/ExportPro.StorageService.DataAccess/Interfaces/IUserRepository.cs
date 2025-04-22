using ExportPro.Auth.SDK.Models;
using ExportPro.StorageService.SDK.DTOs;

namespace ExportPro.StorageService.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken=default);
    }
}
