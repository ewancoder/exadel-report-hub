using System.Security.Claims;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;

namespace ExportPro.AuthService.Services;

public interface IJwtTokenService
{
    AuthResponseDto GenerateToken(User user, List<Claim> claims);
    string GenerateRefreshToken();
}
