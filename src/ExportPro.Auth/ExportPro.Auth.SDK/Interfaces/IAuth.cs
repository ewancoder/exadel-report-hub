using ExportPro.Auth.SDK.DTOs;
using ExportPro.Common.Shared.Library;
using Refit;

namespace ExportPro.Auth.SDK.Interfaces;

public interface IAuth
{
    //todo correct the return types.
    //it works if the model is valid but if it is not than it cant recongize the errors and it bugs 
    //well maybe bcz it only returns string or authresponsedto but the method can return other objects.
    [Post("/api/Auth/register")]
    Task<string> RegisterAsync(UserRegisterDto registerDto);
    [Post("/api/Auth/login")]
    Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
    [Post("/api/Auth/refresh-token")]
    Task<AuthResponseDto> RefreshTokenAsync();
    [Post("/api/Auth/logout")]
    Task<string> LogoutAsync();
}
