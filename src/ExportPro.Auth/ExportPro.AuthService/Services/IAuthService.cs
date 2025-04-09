using ExportPro.Common.Shared.DTOs;
namespace ExportPro.AuthService.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(UserRegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
