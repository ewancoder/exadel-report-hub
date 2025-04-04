using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;

namespace ExportPro.Auth.Services;

public interface IJwtTokenService
{
    AuthResponseDto GenerateToken(User user);
    string GenerateRefreshToken();
}
