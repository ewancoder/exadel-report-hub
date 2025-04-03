using ExportPro.AuthService.Configuration;
using ExportPro.AuthService.Repositories;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace ExportPro.AuthService.Services;

public class AuthService(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<bool> ValidateTokenVersionAsync(ObjectId userId, int tokenVersion)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null && user.TokenVersion == tokenVersion;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await _userRepository.GetByRefreshTokenAsync(refreshToken);
    }

    public async Task<User?> GetUserByIdAsync(ObjectId id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        return await _userRepository.CreateAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateAsync(user);
    }

    public async Task<AuthResponseDto> GenerateTokenAndSetRefreshTokenAsync(User user, HttpContext httpContext)
    {
        user.RefreshTokens.RemoveAll(rt => rt.ExpiresAt <= DateTime.UtcNow);
        AuthResponseDto authResponse = _jwtTokenService.GenerateToken(user);
        string newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        RefreshToken newRefreshToken = new()
        {
            Token = newRefreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            CreatedByIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        };

        user.RefreshTokens.Add(newRefreshToken);
        await _userRepository.UpdateAsync(user);

        // cookie
        httpContext.Response.Cookies.Append("refreshToken", newRefreshTokenValue, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = newRefreshToken.ExpiresAt
        });

        return authResponse;
    }
}