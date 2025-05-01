using ExportPro.Auth.SDK.Models;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.AuthService.Repositories;

public interface IACLRepository
{
    Task<List<UserClientRoles>> GetUserClientRolesAsync(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default);
    Task<List<UserClientRoles>> GetUserRolesAsync(ObjectId userId, CancellationToken cancellationToken = default);
    Task RemoveUserClientRoleAsync(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default);
    Task UpdateUserClientRoleAsync(ObjectId userId, ObjectId clientId, UserRole newRole, CancellationToken cancellationToken = default);
}

