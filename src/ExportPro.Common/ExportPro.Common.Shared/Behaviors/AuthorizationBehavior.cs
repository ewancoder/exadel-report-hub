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
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new UnauthorizedAccessException("User context not found.");

            var userRoleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "Role")?.Value;
            if (!Enum.TryParse<Role>(userRoleClaim, out var userRole))
                throw new UnauthorizedAccessException("Invalid role.");
            if (userRole == Role.SuperAdmin &&
                (permissionedRequest.Resource == Resource.Clients || permissionedRequest.Resource == Resource.Users))
            {
                return await next(cancellationToken);
            }

            var userIdClaim = (user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value) ?? throw new UnauthorizedAccessException("UserId not found in claims.");
            var userId = ObjectId.Parse(userIdClaim);

            var clientId = permissionedRequest.ClientId;
            var hasPermission = await permissionChecker.CheckPermissionAsync(
                new Models.CheckPermissionRequest
                {
                    UserId = userId,
                    ClientId = clientId ?? default,
                    Resource = permissionedRequest.Resource,
                    Action = permissionedRequest.Action
                });

            if (!hasPermission)
                throw new UnauthorizedAccessException($"You do not have permission to perform this action. Action: {permissionedRequest.Action}, Resource: {permissionedRequest.Resource}");
        }

        return await next(cancellationToken);
    }
}

