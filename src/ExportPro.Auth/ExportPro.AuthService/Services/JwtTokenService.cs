using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ExportPro.AuthService.Configuration;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ExportPro.AuthService.Services;

public class JwtTokenService(IOptions<JwtSettings> jwtOptions) : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>A response containing the generated token, the user's username, and the token's expiration date.</returns>
    public AuthResponseDto GenerateToken(User user, List<Claim> claims)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            Username = user.Username,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Generates a secure random refresh token.
    /// </summary>
    /// <returns>A secure random refresh token.</returns>
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
