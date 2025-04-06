using System.Security.Claims;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Models;

namespace ExportPro.AuthService.Services;

public interface IJwtTokenService
{
    AuthResponseDto GenerateAccessToken(User user, List<Claim> claims);
    string GenerateRefreshToken();
}
