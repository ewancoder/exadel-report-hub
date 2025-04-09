using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Library;
using Refit;

namespace ExportPro.Auth.SDK.Interfaces;

public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<BaseResponse<AuthResponseDto>> RegisterAsync([Body] UserRegisterDto registerDto);

    [Post("/api/auth/login")]
    Task<BaseResponse<AuthResponseDto>> LoginAsync([Body] UserLoginDto loginDto);

    [Post("/api/auth/refresh-token")]
    Task<BaseResponse<AuthResponseDto>> RefreshTokenAsync();

    [Post("/api/auth/logout")]
    Task<BaseResponse> LogoutAsync();
}
