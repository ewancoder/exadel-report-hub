using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Repositories;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using MongoDB.Bson;

namespace ExportPro.AuthService.Services;

public sealed class ACLService(IACLRepository aclRepository) : IACLService
{

    public async Task<List<PermissionDTO>> GetPermissions(ObjectId userId, ObjectId clientId = default, CancellationToken cancellationToken = default)
    {
        var userRoles = await Roles(userId, clientId, cancellationToken);
        var permissions = userRoles
                .SelectMany(roleEntry =>
                {
                    if (!RolePermissions.Matrix.TryGetValue(roleEntry.Role, out var rolePermissions))
                        return [];

                    return rolePermissions.SelectMany(permission =>
                        permission.AllowedActions.Select(action => new PermissionDTO
                        {
                            ClientId = roleEntry.ClientId,
                            UserId = roleEntry.UserId,
                            Role = roleEntry.Role,
                            Resource = permission.Resource,
                            Action = action
                        })
                    );
                })
                .ToList();

        return permissions;
    }

    public async Task GrantPermission(ObjectId userId, ObjectId clientId, UserRole role, CancellationToken cancellationToken = default)
    {
        var roleEntry = new UserClientRoles
        {
            UserId = userId,
            ClientId = clientId,
            Role = role
        };

        await aclRepository.AddOneAsync(roleEntry, cancellationToken);
    }

    public async Task<bool> HasPermission(ObjectId userId, ObjectId clientId, Resource resource, CrudAction action, CancellationToken cancellationToken = default)
    {
        var userRoles = await Roles(userId, clientId, cancellationToken);
        foreach (var roleEntry in userRoles)
        {
            if (PermissionChecker.HasPermission(roleEntry.Role, resource, action))
            {
                return true;
            }
        }

        return false;
    }

    public  Task RemovePermission(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default)
    {
         return aclRepository.RemoveUserClientRoleAsync(userId, clientId, cancellationToken);
    }

    public  Task<bool> UpdateUserRole(ObjectId userId, ObjectId clientId, UserRole newRole, CancellationToken cancellationToken = default) => 
        aclRepository.UpdateUserClientRoleAsync(userId, clientId, newRole, cancellationToken);

    public  Task<List<UserClientRoles>> GetAccessibleUserRolesAsync(ObjectId userId, CancellationToken cancellationToken = default) =>
        aclRepository.GetClientIdsForUserAsync(userId, cancellationToken);
 

    private async Task<List<UserClientRoles>> Roles(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken)
    {
        if (clientId != default)
        {
            return await aclRepository.GetUserClientRolesAsync(userId, clientId, cancellationToken);
        }
        return await aclRepository.GetUserRolesAsync(userId, cancellationToken);
    }

    public Task DeleteAllRoles(ObjectId userId, CancellationToken cancellationToken = default) =>
      aclRepository.DeleteRolesAsync(userId, cancellationToken);
}

