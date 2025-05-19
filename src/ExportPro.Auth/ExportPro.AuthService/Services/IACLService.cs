using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Models;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.AuthService.Services;

public interface IACLService
{
    Task<bool> HasPermission(
        ObjectId userId,
        ObjectId? clientId,
        Resource resource,
        CrudAction action,
        CancellationToken cancellationToken = default
    );

    Task<List<PermissionDTO>> GetPermissions(
        ObjectId userId,
        ObjectId clientId = default,
        CancellationToken cancellationToken = default
    );

    Task GrantPermission(
        ObjectId userId,
        ObjectId clientId,
        UserRole role,
        CancellationToken cancellationToken = default
    );
    Task RemovePermission(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserRole(
        ObjectId userId,
        ObjectId clientId,
        UserRole newRole,
        CancellationToken cancellationToken = default
    );
    Task<List<UserClientRoles>> GetAccessibleUserRolesAsync(
        ObjectId userId,
        CancellationToken cancellationToken = default
    );
    Task DeleteAllRoles(ObjectId userId, CancellationToken cancellationToken = default);
}
