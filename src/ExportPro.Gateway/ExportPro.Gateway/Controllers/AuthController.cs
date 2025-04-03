using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.Gateway.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IJwtTokenService jwtTokenService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

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
        var existingUser = await _authService.GetUserByUsernameAsync(registerDto.Username);

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

        user = await _authService.CreateUserAsync(user);

        return Ok(await _authService.GenerateTokenAndSetRefreshTokenAsync(user, HttpContext));
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
        User? user = await _authService.GetUserByUsernameAsync(loginDto.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(await _authService.GenerateTokenAndSetRefreshTokenAsync(user, HttpContext));
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

        User? user = await _authService.GetUserByRefreshTokenAsync(refreshToken);

        if (user == null)
        {
            return Unauthorized("Invalid refresh token.");
        }

        RefreshToken? refreshTokenObject = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (refreshTokenObject == null || refreshTokenObject.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Expired refresh token.");
        }

        return Ok(await _authService.GenerateTokenAndSetRefreshTokenAsync(user, HttpContext));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Ok("User is already logged out.");
        }

        var user = await _authService.GetUserByRefreshTokenAsync(refreshToken);

        if (user != null)
        {
            user.TokenVersion += 1;
            user.RefreshTokens.RemoveAll(rt => rt.Token == refreshToken);
            await _authService.UpdateUserAsync(user);
        }

        Response.Cookies.Delete("refreshToken");

        return Ok("Logged out successfully.");
    }
}