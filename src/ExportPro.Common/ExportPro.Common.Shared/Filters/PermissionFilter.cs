using ExportPro.Common.Shared.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Enums;
using System.Security.Claims;

namespace ExportPro.Common.Shared.Filters;

public class PermissionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        var roleClaim = user?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<Role>(roleClaim, out var userRole))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var endpoint = context.ActionDescriptor.EndpointMetadata;
        var permissionAttributes = endpoint.OfType<HasPermissionAttribute>();

        foreach (var attr in permissionAttributes)
        {
            if (!PermissionChecker.HasPermission(userRole, attr.Resource, attr.Action))
            {
                context.Result = new ObjectResult(new
                {
                    Error = $"No permission for {attr.Resource} - {attr.Action}"
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }

        await next();
    }
}
