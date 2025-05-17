using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExportPro.AuthService.Behaviors;

public class LocalAuthorizationBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor, IACLService service) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IPermissionedRequest permissionedRequest)
        {
            var user = (httpContextAccessor.HttpContext?.User) ?? throw new UnauthorizedAccessException("User context not found.");
            var userRole = TokenHelper.GetUserRole(user);
            if (userRole == Role.JobAdmin)
                return await next(cancellationToken);
            if (userRole == Role.SuperAdmin &&
                (permissionedRequest.Resource == Resource.Clients || permissionedRequest.Resource == Resource.Users))
            {
                return await next(cancellationToken);
            }
            var userId = TokenHelper.GetUserId(user);

            if (permissionedRequest.ClientIds != null && permissionedRequest.ClientIds.Count != 0)
            {
                foreach (var clientId in permissionedRequest.ClientIds)
                {
                    var hasPermission = await service.HasPermission(userId, clientId.ToObjectId(), permissionedRequest.Resource, permissionedRequest.Action, cancellationToken);

                    if (!hasPermission)
                    {
                        throw new UnauthorizedAccessException(
                            $"No permission for client {clientId} - Action: {permissionedRequest.Action}, Resource: {permissionedRequest.Resource}");
                    }
                }
            }
            else
            {
                var hasPermission = await service.HasPermission(userId, default, permissionedRequest.Resource, permissionedRequest.Action, cancellationToken);

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

