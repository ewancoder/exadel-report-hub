using System.Security.Claims;
using ExportPro.Auth.SDK.Models;
using ExportPro.Common.Shared.DTOs;
namespace ExportPro.AuthService.Services;

public interface IJwtTokenService
{
    AuthResponseDto GenerateAccessToken(User user, List<Claim> claims);
    string GenerateRefreshToken();
}
