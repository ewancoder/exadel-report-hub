using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Refit;
using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System.Security.Claims;


namespace ExportPro.Common.Shared.Behaviors;
public class AuthorizationBehavior<TRequest, TResponse>(
    IACLSharedApi permissionChecker,
    IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IPermissionedRequest permissionedRequest)
        {
            var user = (httpContextAccessor.HttpContext?.User) ?? throw new UnauthorizedAccessException("User context not found.");
            var userRole = TokenHelper.GetUserRole(user);
            if (userRole == Role.SuperAdmin &&
                (permissionedRequest.Resource == Resource.Clients || permissionedRequest.Resource == Resource.Users))
            {
                return await next(cancellationToken);
            }
            var userId = TokenHelper.GetUserId(user);

            if (permissionedRequest.ClientIds != null && permissionedRequest.ClientIds.Any())
            {
                foreach (var clientId in permissionedRequest.ClientIds)
                {
                    var hasPermission = await permissionChecker.CheckPermissionAsync(new Models.CheckPermissionRequest
                    {
                        UserId = userId.ToString(),
                        ClientId = clientId,
                        Resource = permissionedRequest.Resource,
                        Action = permissionedRequest.Action
                    });

                    if (!hasPermission)
                    {
                        throw new UnauthorizedAccessException(
                            $"No permission for client {clientId} - Action: {permissionedRequest.Action}, Resource: {permissionedRequest.Resource}");
                    }
                }
            }
            else
            {
                var hasPermission = await permissionChecker.CheckPermissionAsync(new Models.CheckPermissionRequest
                {
                    UserId = userId.ToString(),
                    ClientId = default,
                    Resource = permissionedRequest.Resource,
                    Action = permissionedRequest.Action
                });

                if (!hasPermission)
                {
                    throw new UnauthorizedAccessException(
                        $"No permission - Action: {permissionedRequest.Action}, Resource: {permissionedRequest.Resource}");
                }
            }
        }

        return await next(cancellationToken);
    }
}

