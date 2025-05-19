using System.Security.Claims;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

public static class TokenHelper
{
    public static ObjectId GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user
            ?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")
            ?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedAccessException("UserId not found in claims.");

        if (!ObjectId.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid UserId format.");

        return userId;
    }

    public static Role GetUserRole(ClaimsPrincipal user)
    {
        var roleClaim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "Role")?.Value;

        if (string.IsNullOrWhiteSpace(roleClaim))
            throw new UnauthorizedAccessException("Role not found in claims.");

        if (!Enum.TryParse<Role>(roleClaim, out var role))
            throw new UnauthorizedAccessException("Invalid Role format.");

        return role;
    }
}
