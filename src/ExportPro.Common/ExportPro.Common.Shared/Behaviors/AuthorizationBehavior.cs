using System.Net.Http.Headers;
using System.Text.Json;
using ExportPro.Common.Shared.Helpers;
using ExportPro.Common.Shared.Models;
using ExportPro.Common.Shared.Refit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Refit;



namespace ExportPro.Common.Shared.Behaviors;
public class AuthorizationBehavior<TRequest, TResponse>(
    IACLSharedApi permissionChecker,
    IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IPermissionedRequest permissionedRequest)
        {
            var user = httpContextAccessor.HttpContext?.User ?? throw new UnauthorizedAccessException("User context not found.");
            var url = "https://localhost:7067";
            var jwtToken = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };

            if (!string.IsNullOrEmpty(jwtToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken.Replace("Bearer ", ""));
            }

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    }
                )
            };

            permissionChecker = RestService.For<IACLSharedApi>(httpClient, refitSettings);
            Console.WriteLine(JsonSerializer.Serialize(request));
            await PermissionEvaluator.EnsureHasPermissionAsync(
                permissionedRequest,
                user,
                async (userId, clientId, resource, action, ct) =>
                {
                    var response = await permissionChecker.CheckPermissionAsync(new CheckPermissionRequest
                    {
                        UserId = userId,
                        ClientId = clientId,
                        Resource = resource,
                        Action = action
                    });
                    return response.Data;
                },
                cancellationToken);
        }

        return await next(cancellationToken);
    }
}

