using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.Extensions;
using ExportPro.Common.Shared.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ExportPro.AuthService.Behaviors;

public class LocalAuthorizationBehavior<TRequest, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    IACLService service
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (request is IPermissionedRequest permissionedRequest)
        {
            var user =
                httpContextAccessor.HttpContext?.User
                ?? throw new UnauthorizedAccessException("User context not found.");

            await PermissionEvaluator.EnsureHasPermissionAsync(
                permissionedRequest,
                user,
                (userId, clientId, resource, action, ct) =>
                    service.HasPermission(userId.ToObjectId(), clientId?.ToObjectId() ?? default, resource, action, ct),
                cancellationToken
            );
        }

        return await next(cancellationToken);
    }
}
