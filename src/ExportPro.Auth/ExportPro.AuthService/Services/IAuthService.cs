using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace ExportPro.AuthService.Services;

public interface IAuthService
{
    Task<bool> ValidateTokenVersionAsync(ObjectId userId, int tokenVersion);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task<User?> GetUserByIdAsync(ObjectId id);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<AuthResponseDto> GenerateTokenAndSetRefreshTokenAsync(User user, HttpContext httpContext);
}