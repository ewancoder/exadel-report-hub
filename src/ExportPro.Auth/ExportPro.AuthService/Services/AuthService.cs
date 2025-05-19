using System.Security.Claims;
using ExportPro.Auth.SDK.DTOs;
using ExportPro.Auth.SDK.Models;
using ExportPro.AuthService.Configuration;
using ExportPro.AuthService.Repositories;
using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Exceptions;
using Microsoft.Extensions.Options;

namespace ExportPro.AuthService.Services;

public class AuthService(
    UserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IOptions<JwtSettings> jwtOptions
) : IAuthService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly UserRepository _userRepository = userRepository;

    /// <summary>
    ///     Registers a new user.
    /// </summary>
    /// <param name="dto">The registration data.</param>
    /// <returns>An authentication response containing the JWT token, the user's username, and the token's expiration date.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the email is already registered.</exception>
    public async Task<AuthResponseDto> RegisterAsync(UserRegisterDto dto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);

        if (existingUser != null)
            throw new EmailAlreadyExistsException();
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = Role.None,
        };

        user = await _userRepository.AddOneAsync(user);
        return await GenerateTokenAndSetRefreshToken(user);
    }

    /// <summary>
    ///     Authenticates a user using their email and password.
    /// </summary>
    /// <param name="dto">The login data.</param>
    /// <returns>An authentication response containing the JWT token, the user's username, and the token's expiration date.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the email or password is invalid.</exception>
    public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokenAndSetRefreshToken(user);
    }

    /// <summary>
    ///     Generates a new JWT token if the provided refresh token is valid.
    /// </summary>
    /// <param name="refreshToken">The refresh token to use for generating a new JWT token.</param>
    /// <returns>
    ///     An authentication response containing the generated JWT token, the user's username, and the token's expiration
    ///     date.
    /// </returns>
    /// <exception cref="UnauthorizedAccessException">Thrown if the refresh token is invalid or expired.</exception>
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        var token = user?.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);

        if (user == null || token == null || token.ExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        return await GenerateTokenAndSetRefreshToken(user);
    }

    /// <summary>
    ///     Logs out a user by invalidating the provided refresh token and updating the user's token version.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <remarks>
    ///     This operation removes the refresh token from the user's list of active tokens and increments the user's token
    ///     version to ensure any existing JWT tokens are invalidated.
    /// </remarks>
    public async Task LogoutAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user != null)
        {
            user.RefreshTokens.RemoveAll(rt => rt.Token == refreshToken);
            await _userRepository.UpdateOneAsync(user);
        }
    }

    /// <summary>
    ///     Generates a new JWT token for the given user and sets a new refresh token.
    /// </summary>
    /// <param name="user">The user for whom the tokens are generated.</param>
    /// <returns>
    ///     An authentication response containing the generated JWT token, the user's username, the token's expiration
    ///     date, and the new refresh token.
    /// </returns>
    /// <remarks>
    ///     This method removes expired refresh tokens, generates a new refresh token, updates the user's refresh token list,
    ///     and stores the new token in the database.
    ///     The JWT token is generated with claims including the user's ID, username, role, and token version.
    /// </remarks>
    private async Task<AuthResponseDto> GenerateTokenAndSetRefreshToken(User user)
    {
        user.RefreshTokens.RemoveAll(rt => rt.ExpiresAt <= DateTime.UtcNow);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        RefreshToken newRefreshToken = new()
        {
            Token = newRefreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
        };

        user.RefreshTokens.Add(newRefreshToken);
        await _userRepository.UpdateOneAsync(user);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString()),
        ];

        var accessToken = _jwtTokenService.GenerateAccessToken(user, claims);

        return new AuthResponseDto
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = newRefreshTokenValue,
            Username = user.Username,
            ExpiresAt = accessToken.ExpiresAt,
        };
    }
}
