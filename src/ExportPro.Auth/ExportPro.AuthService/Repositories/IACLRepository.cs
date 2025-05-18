using ExportPro.Auth.SDK.Models;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.AuthService.Repositories;

public interface IACLRepository : IRepository<UserClientRoles>
{
    Task<List<UserClientRoles>> GetUserClientRolesAsync(
        ObjectId userId,
        ObjectId clientId,
        CancellationToken cancellationToken = default
    );

    Task<List<UserClientRoles>> GetUserRolesAsync(ObjectId userId, CancellationToken cancellationToken = default);
    Task RemoveUserClientRoleAsync(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default);

    Task<bool> UpdateUserClientRoleAsync(
        ObjectId userId,
        ObjectId clientId,
        UserRole newRole,
        CancellationToken cancellationToken = default
    );

    Task<List<UserClientRoles>> GetClientIdsForUserAsync(
        ObjectId userId,
        CancellationToken cancellationToken = default
    );
    Task DeleteRolesAsync(ObjectId userId, CancellationToken cancellationToken = default);
}
