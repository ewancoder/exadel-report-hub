using ExportPro.AuthService.Services;
using ExportPro.Common.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.Gateway.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account and returns a JWT token"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] UserRegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            SetRefreshTokenCookie(result);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Login to the system",
        Description = "Authenticates user credentials and returns a JWT token"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        SetRefreshTokenCookie(result);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Refresh JWT token",
        Description = "Generates a new JWT and refresh token if the provided refresh token is valid"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var token))
            return Unauthorized("Refresh token is missing.");

        var result = await _authService.RefreshTokenAsync(token);
        SetRefreshTokenCookie(result);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var token))
        {
            await _authService.LogoutAsync(token);
            Response.Cookies.Delete("refreshToken");
        }

        return Ok("Logged out successfully.");
    }

    private void SetRefreshTokenCookie(AuthResponseDto authResponse)
    {
        Response.Cookies.Append("refreshToken", authResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = authResponse.ExpiresAt
        });
    }
}
