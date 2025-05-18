using System.Security.Claims;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;

namespace ExportPro.Common.Shared.Helpers;

public static class PermissionEvaluator
{
    public static async Task EnsureHasPermissionAsync(
        IPermissionedRequest request,
        ClaimsPrincipal user,
        Func<Guid, Guid?, Resource, CrudAction, CancellationToken, Task<bool>> permissionCheck,
        CancellationToken cancellationToken
    )
    {
        var userRole = TokenHelper.GetUserRole(user);
        if (userRole == Role.JobAdmin)
            return;

        if (userRole == Role.SuperAdmin && (request.Resource == Resource.Clients || request.Resource == Resource.Users))
            return;

        var userId = TokenHelper.GetUserId(user);

        if (request.ClientIds != null && request.ClientIds.Count > 0)
        {
            foreach (var clientId in request.ClientIds)
            {
                var allowed = await permissionCheck(
                    userId.ToGuid(),
                    clientId,
                    request.Resource,
                    request.Action,
                    cancellationToken
                );
                if (!allowed)
                    throw new UnauthorizedAccessException(
                        $"No permission for client {clientId} - Action: {request.Action}, Resource: {request.Resource}"
                    );
            }
        }
        else
        {
            var allowed = await permissionCheck(
                userId.ToGuid(),
                null,
                request.Resource,
                request.Action,
                cancellationToken
            );
            if (!allowed)
                throw new UnauthorizedAccessException(
                    $"No permission - Action: {request.Action}, Resource: {request.Resource}"
                );
        }
    }
}
