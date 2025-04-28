using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Helpers;
using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Repositories;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using MongoDB.Bson;
using System.Threading;

namespace ExportPro.AuthService.Services;

public class ACLService(ACLRepository aclRepository) : IACLService
{

    public async Task<List<PermissionDTO>> GetPermissions(ObjectId userId, ObjectId clientId = default, CancellationToken cancellationToken = default)
    {
        var userRoles = await Roles(userId, clientId, cancellationToken);
        var permissions = userRoles
                .SelectMany(roleEntry =>
                {
                    if (!RolePermissions.Matrix.TryGetValue(roleEntry.Role.ToAuthRole(), out var rolePermissions))
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
            if (PermissionChecker.HasPermission(roleEntry.Role.ToAuthRole(), resource, action))
            {
                return true;
            }
        }

        return false;
    }

    public async Task RemovePermission(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken = default)
    {
         await aclRepository.RemoveUserClientRoleAsync(userId, clientId, cancellationToken);
    }

    public async Task UpdateUserRole(ObjectId userId, ObjectId clientId, UserRole newRole, CancellationToken cancellationToken = default)
    {
        await aclRepository.UpdateUserClientRoleAsync(userId, clientId, newRole, cancellationToken);
    }


    private async Task<List<UserClientRoles>> Roles(ObjectId userId, ObjectId clientId, CancellationToken cancellationToken)
    {
        _ = new List<UserClientRoles>();
        List<UserClientRoles>? userRoles;
        if (clientId != default)
        {
            userRoles = await aclRepository.GetUserClientRolesAsync(userId, clientId, cancellationToken);
        }
        else
        {
            userRoles = await aclRepository.GetUserRolesAsync(userId, cancellationToken);
        }
        return userRoles;
    }

}

