using ExportPro.AuthService.Configuration;
using ExportPro.AuthService.Repositories;
using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.Gateway.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtOptions) : ControllerBase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account and returns a JWT token"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] UserRegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);

        if (existingUser != null)
        {
            return Conflict("Username is already taken");
        }

        User user = new()
        {
            Username = registerDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = UserRole.User
        };

        user = await _userRepository.CreateAsync(user);

        return Ok(await GenerateTokenAndSetRefreshToken(user));
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Login to the system",
        Description = "Authenticates user credentials and returns a JWT token"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto loginDto)
    {
        User? user = await _userRepository.GetByUsernameAsync(loginDto.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(await GenerateTokenAndSetRefreshToken(user));
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Refresh JWT token",
        Description = "Generates a new JWT and refresh token if the provided refresh token is valid"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized("Refresh token is missing.");
        }

        User? user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

        if (user == null)
        {
            return Unauthorized("Invalid refresh token.");
        }

        RefreshToken? refreshTokenObject = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (refreshTokenObject == null || refreshTokenObject.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Expired refresh token.");
        }

        return Ok(await GenerateTokenAndSetRefreshToken(user));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Ok("User is already logged out.");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

        if (user != null)
        {
            user.TokenVersion += 1;
            user.RefreshTokens.RemoveAll(rt => rt.Token == refreshToken);
            await _userRepository.UpdateAsync(user);
        }

        Response.Cookies.Delete("refreshToken");

        return Ok("Logged out successfully.");
    }

    private async Task<AuthResponseDto> GenerateTokenAndSetRefreshToken(User user)
    {
        user.RefreshTokens.RemoveAll(rt => rt.ExpiresAt <= DateTime.UtcNow);
        AuthResponseDto authResponse = _jwtTokenService.GenerateToken(user);
        string newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        RefreshToken newRefreshToken = new()
        {
            Token = newRefreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        };

        user.RefreshTokens.Add(newRefreshToken);
        await _userRepository.UpdateAsync(user);

        // cookie
        Response.Cookies.Append("refreshToken", newRefreshTokenValue, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = newRefreshToken.ExpiresAt
        });

        //// header.
        //Response.Headers.Add("X-Refresh-Token", newRefreshTokenValue);

        return authResponse;
    }
}
