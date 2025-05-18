using ExportPro.Auth.SDK.DTOs;
using ExportPro.Common.Shared.Library;
using Refit;

namespace ExportPro.Auth.SDK.Interfaces;

public interface IAuth
{
    [Post("/api/Auth/register")]
    Task<string> RegisterAsync(UserRegisterDto registerDto);

    [Post("/api/Auth/login")]
    Task<BaseResponse<AuthResponseDto>> LoginAsync(UserLoginDto loginDto);

    [Post("/api/Auth/refresh-token")]
    Task<AuthResponseDto> RefreshTokenAsync();

    [Post("/api/Auth/logout")]
    Task<string> LogoutAsync();
}
